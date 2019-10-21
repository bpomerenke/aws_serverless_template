using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Xunit;
using Amazon.Lambda.Core;
using Amazon.Lambda.TestUtilities;
using Common;
using Common.Models;
using Messages;
using Moq;
using Newtonsoft.Json;
using Version;

namespace Version.Tests
{
    public class LambdaServiceTest
    {    
        [Fact]
        public void GetMessages_ReturnsMessagesFromDb()
        {
            var expectedMessages = new List<Message>();
            var expectedResponse = new APIGatewayProxyResponse();

            var env = new Mock<IEnvironmentWrapper>();
            var responseWrapper = new Mock<IResponseWrapper>();
            
            
            responseWrapper.Setup(x => x.Success(expectedMessages))
                .Returns(expectedResponse);
            
            // Invoke the lambda function and confirm the string was upper cased.
            var testObject = new LambdaService(env.Object, responseWrapper.Object);
            var context = new TestLambdaContext();
            var result = testObject.GetMessages(null, context);

            Assert.Equal(expectedResponse, result);
        }
    }
}
