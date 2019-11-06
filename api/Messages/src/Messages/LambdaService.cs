using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Amazon.ApiGatewayManagementApi;
using Amazon.ApiGatewayManagementApi.Model;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.IotData;
using Amazon.IotData.Model;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.Lambda.DynamoDBEvents;
using Common;
using Common.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Messages
{
    public interface ILambdaService
    {
        Task<APIGatewayProxyResponse> GetMessages(APIGatewayProxyRequest request, ILambdaContext context);
        Task NotifyMessageUpdate(DynamoDBEvent update, ILambdaContext context);
        Task<APIGatewayProxyResponse> PostMessage(APIGatewayProxyRequest request, ILambdaContext context);
    }
    
    public class LambdaService : ILambdaService
    {
        private readonly IEnvironmentWrapper _env;
        private readonly IResponseWrapper _responseWrapper;
        private readonly IDynamoDbContextWrapper _dynamoDbContext;
        private readonly IAmazonApiGatewayManagementApi _apiGatewayManagementApi;
        private readonly IAmazonIotData _amazonIotData;

        public LambdaService(IEnvironmentWrapper env, 
            IResponseWrapper responseWrapper, 
            IDynamoDbContextWrapper dynamoDbContext,
            IAmazonApiGatewayManagementApi apiGatewayManagementApi,
            IAmazonIotData amazonIotData)
        {
            _env = env;
            _responseWrapper = responseWrapper;
            _dynamoDbContext = dynamoDbContext;
            _apiGatewayManagementApi = apiGatewayManagementApi;
            _amazonIotData = amazonIotData;
        }

        public async Task<APIGatewayProxyResponse> GetMessages(APIGatewayProxyRequest request, ILambdaContext context)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var messages = await _dynamoDbContext.ScanAsync<Message>(new ScanCondition[0])
                .GetRemainingAsync(cancellationTokenSource.Token);
            
            return _responseWrapper.Success(messages);
        }
        
        public async Task<APIGatewayProxyResponse> PostMessage(APIGatewayProxyRequest request, ILambdaContext context)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var message = JsonConvert.DeserializeObject<Message>(request.Body);
            
            await _dynamoDbContext.SaveAsync(message, cancellationTokenSource.Token);
            await _amazonIotData.PublishAsync(new PublishRequest
            {
                Topic = $"CHAT/Messages",
                Qos = 1,
                Payload = new MemoryStream(
                    Encoding.UTF8.GetBytes(request.Body))
            }, cancellationTokenSource.Token);

            return _responseWrapper.Success(message);
        }

        public async Task NotifyMessageUpdate(DynamoDBEvent update, ILambdaContext context)
        {
            context.Logger.LogLine("notifying message");
            var cancellationTokenSource = new CancellationTokenSource();
            var connections = await _dynamoDbContext.QueryAsync<WebSocketConnection>("true")
                .GetRemainingAsync(cancellationTokenSource.Token);

            foreach (var record in update.Records)
            {
                var jsonResult = Document.FromAttributeMap(record.Dynamodb.NewImage).ToJson();
                var message = JsonConvert.DeserializeObject<Message>(jsonResult);
                foreach (var connection in connections)
                {
                    try
                    {
                        context.Logger.LogLine($"sending update to {connection.ConnectionId}...");
                        var settings = new JsonSerializerSettings
                        {
                            ContractResolver = new CamelCasePropertyNamesContractResolver()
                        };
                        var payload = JsonConvert.SerializeObject(message, settings);
                        await _apiGatewayManagementApi.PostToConnectionAsync(new PostToConnectionRequest
                        {
                            ConnectionId = connection.ConnectionId,
                            Data = new MemoryStream(Encoding.UTF8.GetBytes(payload))
                        }, cancellationTokenSource.Token);
                        context.Logger.LogLine($"update sent");
                    }
                    catch (Exception e)
                    {
                        context.Logger.LogLine($"failed to send to {connection.ConnectionId}...moving on");
                        context.Logger.LogLine(e.ToString());
                    }
                }
            }

            
        }

    }
}