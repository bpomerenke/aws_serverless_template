using System;
using System.Collections.Generic;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Version
{
    public interface ILambdaService
    {
        APIGatewayProxyResponse GetVersion(APIGatewayProxyRequest request, ILambdaContext context);
    }
    
    public class LambdaService : ILambdaService
    {
        private readonly IEnvironmentWrapper _env;

        public LambdaService(IEnvironmentWrapper env)
        {
            _env = env;
        }

        public APIGatewayProxyResponse GetVersion(APIGatewayProxyRequest request, ILambdaContext context)
        {
            var versionInfo = new VersionInfo
            {
                Version = _env.GetEnvironmentVariable("Version")
            };
            return new APIGatewayProxyResponse
            {
                StatusCode = 200,
                Body = JsonConvert.SerializeObject(versionInfo, new JsonSerializerSettings{ ContractResolver = new CamelCasePropertyNamesContractResolver() }),
                Headers = new Dictionary<string, string>
                {
                    {"Access-Control-Allow-Origin", Environment.GetEnvironmentVariable("CORSAllowedOrigin")}
                }
            };
        }
    }
}