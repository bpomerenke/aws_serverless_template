using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Common;
using Microsoft.Extensions.DependencyInjection;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace Messages
{
    public class Function
    {
        private readonly ServiceProvider _serviceProvider;

        private void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IEnvironmentWrapper, EnvironmentWrapper>();
            serviceCollection.AddTransient<ILambdaService, LambdaService>();
            serviceCollection.AddTransient<IResponseWrapper, ResponseWrapper>();            
            serviceCollection.AddTransient<IDynamoDBContext, DynamoDBContext>(x => DynamoDbConfig.CreateConfiguredDbContext());
        }

        public Function()
        { 
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            _serviceProvider = serviceCollection.BuildServiceProvider();
        }
        
        public APIGatewayProxyResponse FunctionHandler(APIGatewayProxyRequest request, ILambdaContext context)
        {
            return _serviceProvider
                .GetService<ILambdaService>()
                .GetMessages(request, context);
        }
    }
}
