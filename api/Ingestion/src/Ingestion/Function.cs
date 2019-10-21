using System;
using System.Threading.Tasks;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.Core;
using Amazon.Util;
using Common;
using Common.Models;
using Microsoft.Extensions.DependencyInjection;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace Ingestion
{
    public class Function
    {
             
        private readonly ServiceProvider _serviceProvider;

        private void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IEnvironmentWrapper, EnvironmentWrapper>();
            serviceCollection.AddTransient<ILambdaService, LambdaService>();
            serviceCollection.AddTransient<IDynamoDBContext, DynamoDBContext>(x => CreateConfiguredDbContext());
        }

        private DynamoDBContext CreateConfiguredDbContext()
        {            
            var messagesTableName = Environment.GetEnvironmentVariable("MessagesTableName");
            if (!string.IsNullOrEmpty(messagesTableName))
            {
                AWSConfigsDynamoDB.Context.TypeMappings[typeof(Message)] = new TypeMapping(typeof(Message), messagesTableName);
            }

            var v2Config = new DynamoDBOperationConfig {Conversion = DynamoDBEntryConversion.V2};
            return new DynamoDBContext(new AmazonDynamoDBClient(), v2Config);
        }

        public Function()
        { 
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            _serviceProvider = serviceCollection.BuildServiceProvider();
        }
        /// <summary>
        /// Ingests a Message from IOT
        /// </summary>
        /// <param name="message"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task IngestMessage(Message message, ILambdaContext context)
        {
            return _serviceProvider
                .GetService<ILambdaService>()
                .IngestMessage(message, context);
        }
    }
}
