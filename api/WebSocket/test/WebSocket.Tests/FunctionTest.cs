using Amazon.Lambda.APIGatewayEvents;
using Xunit;
using Amazon.Lambda.TestUtilities;

namespace WebSocket.Tests
{
    public class FunctionTest
    {
        [Fact]
        public void Connect_Returns200()
        {

            // Invoke the lambda function and confirm the string was upper cased.
            var function = new Function();
            var context = new TestLambdaContext();
            var response = function.Connect(new APIGatewayProxyRequest(), context);

            Assert.Equal(200, response.StatusCode);
        }
        
        [Fact]
        public void Disconnect_Returns200()
        {
            // Invoke the lambda function and confirm the string was upper cased.
            var function = new Function();
            var context = new TestLambdaContext();
            var response = function.Disconnect(new APIGatewayProxyRequest(), context);

            Assert.Equal(200, response.StatusCode);
        }
    }
}
