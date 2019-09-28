using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

            var env = new Mock<IEnvironmentWrapper>();
            env.Setup(x => x.GetEnvironmentVariable("Version")).Returns(expectedVersion);
            
            // Invoke the lambda function and confirm the string was upper cased.
            var testObject = new LambdaService(env.Object);
            var context = new TestLambdaContext();
            var result = testObject.GetVersion(null, context);

            Assert.Equal(200, result.StatusCode);

            var versionInfo = JsonConvert.DeserializeObject<VersionInfo>(result.Body);
            Assert.Equal(expectedVersion, versionInfo.Version);
        }
    }
}
