using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Xunit;
using Amazon.Lambda.Core;
using Amazon.Lambda.TestUtilities;
using Newtonsoft.Json;
using Version;

namespace Version.Tests
{
    public class FunctionTest
    {
        [Fact]
        public void TestVersion()
        {

            // Invoke the lambda function and confirm the string was upper cased.
            var function = new Function();
            var context = new TestLambdaContext();
            var result = function.FunctionHandler(null, context);

            Assert.Equal(200, result.StatusCode);

            var versionInfo = JsonConvert.DeserializeObject<VersionInfo>(result.Body);
            Assert.Equal("0.1", versionInfo.Version);
        }
    }
}
