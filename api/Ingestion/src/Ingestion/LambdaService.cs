using System.Threading.Tasks;
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

        public LambdaService(IEnvironmentWrapper env)
        {
            _env = env;
        }

        public async Task IngestMessage(Message message, ILambdaContext context)
        {
            context.Logger.LogLine("got message: " + JsonConvert.SerializeObject(message));
        }
    }
}