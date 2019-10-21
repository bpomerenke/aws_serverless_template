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
        private readonly IResponseWrapper _responseWrapper;

        public LambdaService(IEnvironmentWrapper env, IResponseWrapper responseWrapper)
        {
            _env = env;
            _responseWrapper = responseWrapper;
        }

        public APIGatewayProxyResponse GetVersion(APIGatewayProxyRequest request, ILambdaContext context)
        {
            var versionInfo = new VersionInfo
            {
                Version = _env.GetEnvironmentVariable("Version")
            };

            return _responseWrapper.Success(versionInfo);
        }
    }
}