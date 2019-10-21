using System.Collections.Generic;
using System.Threading;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.TestUtilities;
using Common;
using Common.Models;
using Moq;
using Xunit;

namespace Messages.Tests
{
    public class LambdaServiceTest
    {    
        [Fact]
        public async void GetMessages_ReturnsMessagesFromDb()
        {
            var expectedMessages = new List<Message>
            {
                new Message{ ClientId = "some client", MsgText = "Hello there"}
            };
            var expectedResponse = new APIGatewayProxyResponse();

            var env = new Mock<IEnvironmentWrapper>();
            var responseWrapper = new Mock<IResponseWrapper>();
            var dynamoDbContextWrapper = new Mock<IDynamoDbContextWrapper>();

            var asyncSearch = new Mock<IAsyncSearchWrapper<Message>>();
            asyncSearch.Setup(x => x.GetRemainingAsync(It.IsAny<CancellationToken>())).ReturnsAsync(expectedMessages);
            dynamoDbContextWrapper.Setup(x => x.ScanAsync<Message>(It.IsAny<ScanCondition[]>(), null))
                .Returns(asyncSearch.Object);
            responseWrapper.Setup(x => x.Success(expectedMessages))
                .Returns(expectedResponse);
            
            // Invoke the lambda function and confirm the string was upper cased.
            var testObject = new LambdaService(env.Object, responseWrapper.Object, dynamoDbContextWrapper.Object);
            var context = new TestLambdaContext();
            var result = await testObject.GetMessages(null, context);

            Assert.Equal(expectedResponse, result);
        }
    }
}
