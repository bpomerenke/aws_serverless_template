using System.Threading.Tasks;
using Amazon.ApiGatewayManagementApi;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Common;
using Microsoft.Extensions.DependencyInjection;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace WebSocket
{
    public class Function
    {
        private readonly ServiceProvider _serviceProvider;

        private void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IEnvironmentWrapper, EnvironmentWrapper>();
            serviceCollection.AddTransient<ILambdaService, LambdaService>();

            serviceCollection.AddTransient<IDynamoDbContextWrapper, DynamoDbContextWrapper>(
                x => DynamoDbConfig.CreateConfiguredDbContextWrapper());
        }

        public Function()
        { 
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            _serviceProvider = serviceCollection.BuildServiceProvider();
        }
        public Task<APIGatewayProxyResponse> Connect(APIGatewayProxyRequest request, ILambdaContext context)
        {
            return _serviceProvider
                .GetService<ILambdaService>()
                .Connect(request, context);
        }
        public Task<APIGatewayProxyResponse> Disconnect(APIGatewayProxyRequest request, ILambdaContext context)
        {
            return _serviceProvider
                .GetService<ILambdaService>()
                .Disconnect(request, context);
        }
    }
}
