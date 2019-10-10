using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.Core;
using Ingestion.Models;
using Newtonsoft.Json;

namespace Ingestion
{
    public interface ILambdaService
    {
        Task IngestMessage(Message message, ILambdaContext context);
    }
    
    public class LambdaService : ILambdaService
    {
        private readonly IEnvironmentWrapper _env;
        private readonly IDynamoDBContext _dynamoDbContext;

        public LambdaService(IEnvironmentWrapper env, IDynamoDBContext dynamoDbContext)
        {
            _env = env;
            _dynamoDbContext = dynamoDbContext;
        }

        public async Task IngestMessage(Message message, ILambdaContext context)
        {
            context.Logger.LogLine("got message: " + JsonConvert.SerializeObject(message));
            
            var cancellationTokenSource = new CancellationTokenSource();
            await _dynamoDbContext.SaveAsync(message, cancellationTokenSource.Token);
        }
    }
}