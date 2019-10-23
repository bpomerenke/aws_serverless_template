using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Amazon.ApiGatewayManagementApi;
using Amazon.ApiGatewayManagementApi.Model;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Common;
using Common.Models;
using Newtonsoft.Json;

namespace WebSocket
{
    public interface ILambdaService
    {
        Task<APIGatewayProxyResponse> Connect(APIGatewayProxyRequest request, ILambdaContext context);
        Task<APIGatewayProxyResponse> Disconnect(APIGatewayProxyRequest request, ILambdaContext context);
    }
    
    public class LambdaService : ILambdaService
    {
        private readonly IDynamoDbContextWrapper _dynamoDbContext;

        public LambdaService(IDynamoDbContextWrapper dynamoDbContext)
        {
            _dynamoDbContext = dynamoDbContext;
        }
        
        public async Task<APIGatewayProxyResponse> Connect(APIGatewayProxyRequest request, ILambdaContext context)
        {
            context.Logger.Log($"connected: {request.RequestContext?.ConnectionId}");
            await _dynamoDbContext.SaveAsync(new WebSocketConnection
            {
                ConnectionId = request.RequestContext.ConnectionId,
                Connected = "true"
            });
            
            return new APIGatewayProxyResponse
            {
                StatusCode = 200
            };
        }
        public async Task<APIGatewayProxyResponse> Disconnect(APIGatewayProxyRequest request, ILambdaContext context)
        {
            context.Logger.Log($"disconnected: {request.RequestContext?.ConnectionId}");
            await _dynamoDbContext.SaveAsync(new WebSocketConnection
            {
                ConnectionId = request.RequestContext.ConnectionId,
                Connected = "false"
            });
            
            return new APIGatewayProxyResponse
            {
                StatusCode = 200
            };
        }

    }
}