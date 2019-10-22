using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace WebSocket
{
    public class Function
    {
        public APIGatewayProxyResponse Connect(APIGatewayProxyRequest request, ILambdaContext context)
        {
            context.Logger.Log($"connected: {request.RequestContext?.ConnectionId}");
            return new APIGatewayProxyResponse
            {
                StatusCode = 200
            };
        }
        public APIGatewayProxyResponse Disconnect(APIGatewayProxyRequest request, ILambdaContext context)
        {
            context.Logger.Log($"disconnected: {request.RequestContext?.ConnectionId}");
            return new APIGatewayProxyResponse
            {
                StatusCode = 200
            };
        }
    }
}
