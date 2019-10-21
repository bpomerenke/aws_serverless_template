using System.Collections.Generic;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Common;
using Common.Models;

namespace Messages
{
    public interface ILambdaService
    {
        APIGatewayProxyResponse GetMessages(APIGatewayProxyRequest request, ILambdaContext context);
    }
    
    public class LambdaService : ILambdaService
    {
        private readonly IEnvironmentWrapper _env;
        private readonly IResponseWrapper _responseWrapper;

        public LambdaService(IEnvironmentWrapper env, IResponseWrapper responseWrapper)
        {
            _env = env;
            _responseWrapper = responseWrapper;
        }

        public APIGatewayProxyResponse GetMessages(APIGatewayProxyRequest request, ILambdaContext context)
        {
            var messages = new List<Message>();
            
            return _responseWrapper.Success(messages);
        }
    }
}