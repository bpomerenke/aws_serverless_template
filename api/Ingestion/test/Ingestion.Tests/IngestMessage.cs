using Xunit;
using Amazon.Lambda.TestUtilities;
using Ingestion.Models;

namespace Ingestion.Tests
{
    public class IngestMessage
    {
        [Fact]
        public async void DoesStuff()
        {
            // Invoke the lambda function and confirm the string was upper cased.
            var testObject = new LambdaService(new EnvironmentWrapper());
            var context = new TestLambdaContext();
            
            await testObject.IngestMessage(new Message(), context);
        }
    }
}
