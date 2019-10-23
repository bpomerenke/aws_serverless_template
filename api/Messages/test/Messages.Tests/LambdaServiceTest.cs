using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Amazon.ApiGatewayManagementApi;
using Amazon.ApiGatewayManagementApi.Model;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.DynamoDBEvents;
using Amazon.Lambda.TestUtilities;
using Common;
using Common.Models;
using Moq;
using Newtonsoft.Json;
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

        private DynamoDBEvent CreateDynamoEvent(Message message)
        {
            var document = Document.FromJson(JsonConvert.SerializeObject(message));
            return new DynamoDBEvent
            {
                Records = new List<DynamoDBEvent.DynamodbStreamRecord>
                {
                    new DynamoDBEvent.DynamodbStreamRecord
                    {
                        Dynamodb = new StreamRecord
                        {
                            NewImage = document.ToAttributeMap()
                        }
                    }
                }
            };
        }
        
        [Fact]
        public async void NotifyMessageUpdates_SendsToOpenConnections()
        {
            var update = CreateDynamoEvent(new Message());

            var connectionA = new WebSocketConnection { ConnectionId = "some connection A" };
            var connectionB = new WebSocketConnection { ConnectionId = "some connection B" };

            var expectedWebSocketConnections = new List<WebSocketConnection>
            {
                connectionA, connectionB
            };
            
            var asyncSearch = new Mock<IAsyncSearchWrapper<WebSocketConnection>>();
            asyncSearch.Setup(x => x.GetRemainingAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedWebSocketConnections);
            _dynamoDbContextWrapper.Setup(x => x.QueryAsync<WebSocketConnection>("true", null))
                .Returns(asyncSearch.Object);
            
            await _testObject.NotifyMessageUpdate(update, new TestLambdaContext());
            
            _apiGatewayManagementApi.Verify(x=>x.PostToConnectionAsync(
                It.Is<PostToConnectionRequest>(p => p.ConnectionId == connectionA.ConnectionId), 
                It.IsAny<CancellationToken>()), Times.Once);
            _apiGatewayManagementApi.Verify(x=>x.PostToConnectionAsync(
                It.Is<PostToConnectionRequest>(p => p.ConnectionId == connectionB.ConnectionId), 
                It.IsAny<CancellationToken>()), Times.Once);
        }
        
        [Fact]
        public async void NotifyMessageUpdates_SendsProperMessage()
        {
            var message = new Message
            {
                ClientId = "foo",
                MsgText = "bar"
            };

            var update = CreateDynamoEvent(message);
            var expectedWebSocketConnections = new List<WebSocketConnection>
            {
                new WebSocketConnection
                {
                    ConnectionId = "some connection id"
                }
            };
            
            var asyncSearch = new Mock<IAsyncSearchWrapper<WebSocketConnection>>();
            asyncSearch.Setup(x => x.GetRemainingAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedWebSocketConnections);
            _dynamoDbContextWrapper.Setup(x => x.QueryAsync<WebSocketConnection>("true", null))
                .Returns(asyncSearch.Object);

            PostToConnectionRequest postRequest = null;
            _apiGatewayManagementApi.Setup(x => x.PostToConnectionAsync(
                    It.IsAny<PostToConnectionRequest>(),
                    It.IsAny<CancellationToken>()))
                .Callback<PostToConnectionRequest, CancellationToken>((r, _) => postRequest = r)
                .ReturnsAsync(new PostToConnectionResponse());
            
            await _testObject.NotifyMessageUpdate(update, new TestLambdaContext());

            var data = Encoding.UTF8.GetString(postRequest.Data.ToArray());
            Assert.Equal(JsonConvert.SerializeObject(message), data);
        }
        
        [Fact]
        public async void NotifyMessageUpdates_MovesOnWhenErrorsHappenSendingToWebsocket()
        {
            var message = new Message
            {
                ClientId = "foo",
                MsgText = "bar"
            };

            var update = CreateDynamoEvent(message);
            var expectedWebSocketConnections = new List<WebSocketConnection>
            {
                new WebSocketConnection
                {
                    ConnectionId = "some connection id"
                }
            };
            
            var asyncSearch = new Mock<IAsyncSearchWrapper<WebSocketConnection>>();
            asyncSearch.Setup(x => x.GetRemainingAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedWebSocketConnections);
            _dynamoDbContextWrapper.Setup(x => x.QueryAsync<WebSocketConnection>("true", null))
                .Returns(asyncSearch.Object);

            _apiGatewayManagementApi.Setup(x => x.PostToConnectionAsync(
                    It.IsAny<PostToConnectionRequest>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception());
            
            await _testObject.NotifyMessageUpdate(update, new TestLambdaContext());
        }
    }
}
