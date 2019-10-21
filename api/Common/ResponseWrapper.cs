using System;
using System.Collections.Generic;
using Amazon.Lambda.APIGatewayEvents;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Common
{
    public interface IResponseWrapper
    {
        APIGatewayProxyResponse Success(object responseObj);
    }
   
    public class ResponseWrapper : IResponseWrapper
    {
        public APIGatewayProxyResponse Success(object responseObj)
        {
            return new APIGatewayProxyResponse
            {
                StatusCode = 200,
                Body = JsonConvert.SerializeObject(responseObj, new JsonSerializerSettings{ ContractResolver = new CamelCasePropertyNamesContractResolver() }),
                Headers = new Dictionary<string, string>
                {
                    {"Access-Control-Allow-Origin", Environment.GetEnvironmentVariable("CORSAllowedOrigin")}
                }
            };
        }
    }
}