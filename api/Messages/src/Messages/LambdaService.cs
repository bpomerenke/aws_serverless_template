using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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
    }
    
    public class LambdaService : ILambdaService
    {
        private readonly IEnvironmentWrapper _env;
        private readonly IResponseWrapper _responseWrapper;
        private readonly IDynamoDbContextWrapper _dynamoDbContext;

        public LambdaService(IEnvironmentWrapper env, IResponseWrapper responseWrapper, IDynamoDbContextWrapper dynamoDbContext)
        {
            _env = env;
            _responseWrapper = responseWrapper;
            _dynamoDbContext = dynamoDbContext;
        }

        public async Task<APIGatewayProxyResponse> GetMessages(APIGatewayProxyRequest request, ILambdaContext context)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var messages = await _dynamoDbContext.ScanAsync<Message>(new ScanCondition[0])
                .GetRemainingAsync(cancellationTokenSource.Token);
            
            return _responseWrapper.Success(messages);
        }
    }
}