using System;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Util;
using Common.Models;

namespace Common
{
    public static class DynamoDbConfig
    {
        public static DynamoDBContext CreateConfiguredDbContext()
        {            
            return new DynamoDBContext(new AmazonDynamoDBClient(),  GetConfig());
        }

        public static DynamoDbContextWrapper CreateConfiguredDbContextWrapper()
        {
            return new DynamoDbContextWrapper(new AmazonDynamoDBClient(),  GetConfig());
        }

        private static DynamoDBOperationConfig GetConfig()
        {
            var messagesTableName = Environment.GetEnvironmentVariable("MessagesTableName");
            if (!string.IsNullOrEmpty(messagesTableName))
            {
                AWSConfigsDynamoDB.Context.TypeMappings[typeof(Message)] = new TypeMapping(typeof(Message), messagesTableName);
            }
            
            var webSocketConnectionsTableName = Environment.GetEnvironmentVariable("WebSocketConnectionsTableName");
            if (!string.IsNullOrEmpty(webSocketConnectionsTableName))
            {
                AWSConfigsDynamoDB.Context.TypeMappings[typeof(WebSocketConnection)] = 
                    new TypeMapping(typeof(WebSocketConnection), webSocketConnectionsTableName);
            }

            var v2Config = new DynamoDBOperationConfig {Conversion = DynamoDBEntryConversion.V2};
            return v2Config;
        }
    }
}