using System.Threading;
using Amazon.DynamoDBv2.DataModel;
using Xunit;
using Amazon.Lambda.TestUtilities;
using Ingestion.Models;
using Moq;

namespace Ingestion.Tests
{
    public class IngestMessage
    {
        [Fact]
        public async void SavesMessage()
        {
            // Invoke the lambda function and confirm the string was upper cased.
            var dynamoDbContext = new Mock<IDynamoDBContext>();
            
            var testObject = new LambdaService(new EnvironmentWrapper(), dynamoDbContext.Object);
            var context = new TestLambdaContext();

            var message = new Message();
            await testObject.IngestMessage(message, context);
            
            dynamoDbContext.Verify(x=>x.SaveAsync(message, It.IsAny<CancellationToken>()));
        }
    }
}
