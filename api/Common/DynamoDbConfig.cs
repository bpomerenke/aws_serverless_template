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
            var messagesTableName = Environment.GetEnvironmentVariable("MessagesTableName");
            if (!string.IsNullOrEmpty(messagesTableName))
            {
                AWSConfigsDynamoDB.Context.TypeMappings[typeof(Message)] = new TypeMapping(typeof(Message), messagesTableName);
            }

            var v2Config = new DynamoDBOperationConfig {Conversion = DynamoDBEntryConversion.V2};
            return new DynamoDBContext(new AmazonDynamoDBClient(), v2Config);
        }
    }
}