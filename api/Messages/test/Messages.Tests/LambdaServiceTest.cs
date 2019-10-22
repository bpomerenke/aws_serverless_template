using System.Collections.Generic;
using System.Threading;
using Amazon.ApiGatewayManagementApi;
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
        readonly Mock<IEnvironmentWrapper> _env = new Mock<IEnvironmentWrapper>();
        readonly Mock<IResponseWrapper> _responseWrapper = new Mock<IResponseWrapper>();
        readonly Mock<IDynamoDbContextWrapper> _dynamoDbContextWrapper = new Mock<IDynamoDbContextWrapper>();
        readonly Mock<IAmazonApiGatewayManagementApi> _apiGatewayManagementApi = new Mock<IAmazonApiGatewayManagementApi>();

        readonly LambdaService _testObject;

        public LambdaServiceTest()
        {
            _testObject = new LambdaService(_env.Object, _responseWrapper.Object, _dynamoDbContextWrapper.Object, _apiGatewayManagementApi.Object);
        }
        
        [Fact]
        public async void GetMessages_ReturnsMessagesFromDb()
        {
            var expectedMessages = new List<Message>
            {
                new Message{ ClientId = "some client", MsgText = "Hello there"}
            };
            var expectedResponse = new APIGatewayProxyResponse();


            var asyncSearch = new Mock<IAsyncSearchWrapper<Message>>();
            asyncSearch.Setup(x => x.GetRemainingAsync(It.IsAny<CancellationToken>())).ReturnsAsync(expectedMessages);
            _dynamoDbContextWrapper.Setup(x => x.ScanAsync<Message>(It.IsAny<ScanCondition[]>(), null))
                .Returns(asyncSearch.Object);
            _responseWrapper.Setup(x => x.Success(expectedMessages))
                .Returns(expectedResponse);
            
            // Invoke the lambda function and confirm the string was upper cased.
            var context = new TestLambdaContext();
            var result = await _testObject.GetMessages(null, context);

            Assert.Equal(expectedResponse, result);
        }
    }
}
