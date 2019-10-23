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

namespace WebSocket.Tests
{
    public class LambdaServiceTest
    {
        readonly Mock<IDynamoDbContextWrapper> _dynamoDbContextWrapper = new Mock<IDynamoDbContextWrapper>();

        readonly TestLambdaContext _context = new TestLambdaContext();
        readonly LambdaService _testObject;

        public LambdaServiceTest()
        {
            _testObject = new LambdaService(_dynamoDbContextWrapper.Object);
        }
        
        [Fact]
        public async void Connect_SavesRecordWithConnectedTrue()
        {
            var expectedConnectionId = "some id here";
            
            var request = new APIGatewayProxyRequest
            {
                RequestContext = new APIGatewayProxyRequest.ProxyRequestContext
                {
                    ConnectionId = expectedConnectionId
                }
            };
            
            await _testObject.Connect(request, _context);

            _dynamoDbContextWrapper.Verify(x => x.SaveAsync(
                It.Is<WebSocketConnection>(wsc=>wsc.ConnectionId == expectedConnectionId && wsc.Connected == "true"), 
                It.IsAny<CancellationToken>()), Times.Once);
        }
        
        [Fact]
        public async void Disconnect_SavesRecordWithConnectedFalse()
        {
            var expectedConnectionId = "some id here";
            
            var request = new APIGatewayProxyRequest
            {
                RequestContext = new APIGatewayProxyRequest.ProxyRequestContext
                {
                    ConnectionId = expectedConnectionId
                }
            };
            
            await _testObject.Disconnect(request, _context);

            _dynamoDbContextWrapper.Verify(x => x.SaveAsync(
                It.Is<WebSocketConnection>(wsc=>wsc.ConnectionId == expectedConnectionId && wsc.Connected == "false"), 
                It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
