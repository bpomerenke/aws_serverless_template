using System;
using Amazon.IotData;

namespace Common
{
    public static class IotConfig
    {
        public static AmazonIotDataClient CreateAmazonIotDataClient()
        {
            var mqttBrokerEndpoint = Environment.GetEnvironmentVariable("MQTTBroker");
            var clientConfig = new AmazonIotDataConfig {ServiceURL = mqttBrokerEndpoint};
            return new AmazonIotDataClient(clientConfig);
        }

    }
}