using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Common;
using Common.Models;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace WebSocket
{
    public class Function
    {
        public async Task<APIGatewayProxyResponse> Connect(APIGatewayProxyRequest request, ILambdaContext context)
        {
            context.Logger.Log($"connected: {request.RequestContext?.ConnectionId}");

            using (var dynamoDbContext = DynamoDbConfig.CreateConfiguredDbContextWrapper())
            {
                await dynamoDbContext.SaveAsync(new WebSocketConnection
                {
                    ConnectionId = request.RequestContext.ConnectionId,
                    Connected = "true"
                });
            }
            return new APIGatewayProxyResponse
            {
                StatusCode = 200
            };
        }
        public async Task<APIGatewayProxyResponse> Disconnect(APIGatewayProxyRequest request, ILambdaContext context)
        {
            context.Logger.Log($"disconnected: {request.RequestContext?.ConnectionId}");
            using (var dynamoDbContext = DynamoDbConfig.CreateConfiguredDbContextWrapper())
            {
                await dynamoDbContext.SaveAsync(new WebSocketConnection
                {
                    ConnectionId = request.RequestContext.ConnectionId,
                    Connected = "false"
                });
            }
            
            return new APIGatewayProxyResponse
            {
                StatusCode = 200
            };
        }
    }
}
