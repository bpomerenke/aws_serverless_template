using System.Threading;
using System.Threading.Tasks;
using Amazon.ApiGatewayManagementApi;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Common;
using Common.Models;

namespace Messages
{
    public interface ILambdaService
    {
        Task<APIGatewayProxyResponse> GetMessages(APIGatewayProxyRequest request, ILambdaContext context);
        Task NotifyMessageUpdate(object input, ILambdaContext context);
    }
    
    public class LambdaService : ILambdaService
    {
        private readonly IEnvironmentWrapper _env;
        private readonly IResponseWrapper _responseWrapper;
        private readonly IDynamoDbContextWrapper _dynamoDbContext;
        private readonly IAmazonApiGatewayManagementApi _apiGatewayManagementApi;

        public LambdaService(IEnvironmentWrapper env, 
            IResponseWrapper responseWrapper, 
            IDynamoDbContextWrapper dynamoDbContext,
            IAmazonApiGatewayManagementApi apiGatewayManagementApi)
        {
            _env = env;
            _responseWrapper = responseWrapper;
            _dynamoDbContext = dynamoDbContext;
            _apiGatewayManagementApi = apiGatewayManagementApi;
        }

        public async Task<APIGatewayProxyResponse> GetMessages(APIGatewayProxyRequest request, ILambdaContext context)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var messages = await _dynamoDbContext.ScanAsync<Message>(new ScanCondition[0])
                .GetRemainingAsync(cancellationTokenSource.Token);
            
            return _responseWrapper.Success(messages);
        }

        public Task NotifyMessageUpdate(object input, ILambdaContext context)
        {
            context.Logger.LogLine("notifying message");
            return Task.CompletedTask;
        }
    }
}