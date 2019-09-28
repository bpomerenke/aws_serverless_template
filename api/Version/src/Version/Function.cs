using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Microsoft.Extensions.DependencyInjection;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace Version
{
    public class VersionInfo
    {
        public string Version { get; set; }
    }
    public class Function
    {        
        private readonly ServiceProvider _serviceProvider;

        private void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IEnvironmentWrapper, EnvironmentWrapper>();
            serviceCollection.AddTransient<ILambdaService, LambdaService>();
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
                .GetVersion(request, context);

        }
    }
}
