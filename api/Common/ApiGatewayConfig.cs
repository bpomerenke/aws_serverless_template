using System;
using Amazon.ApiGatewayManagementApi;

namespace Common
{
    public static class ApiGatewayConfig
    {
        public static AmazonApiGatewayManagementApiClient CreateConfiguredApiGatewayManagementApiClient()
        {
            var serviceUrl = Environment.GetEnvironmentVariable("ServiceURL");
            if (string.IsNullOrEmpty(serviceUrl)) return null;
            var config = new AmazonApiGatewayManagementApiConfig {ServiceURL = serviceUrl};
            return new AmazonApiGatewayManagementApiClient(config);
        }

    }
}