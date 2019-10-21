using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Xunit;
using Amazon.Lambda.Core;
using Amazon.Lambda.TestUtilities;
using Moq;
using Newtonsoft.Json;
using Version;

namespace Version.Tests
{
    public class LambdaServiceTest
    {    
        [Fact]
        public void GetVersion_ReturnsVersionFromEnv()
        {
            var expectedVersion = "abc";
            var expectedResponse = new APIGatewayProxyResponse();

            var env = new Mock<IEnvironmentWrapper>();
            var responseWrapper = new Mock<IResponseWrapper>();
            
            env.Setup(x => x.GetEnvironmentVariable("Version")).Returns(expectedVersion);
            responseWrapper.Setup(x => x.Success(It.Is<VersionInfo>(v => v.Version == expectedVersion)))
                .Returns(expectedResponse);
            
            // Invoke the lambda function and confirm the string was upper cased.
            var testObject = new LambdaService(env.Object, responseWrapper.Object);
            var context = new TestLambdaContext();
            var result = testObject.GetVersion(null, context);

            Assert.Equal(expectedResponse, result);
        }
    }
}
