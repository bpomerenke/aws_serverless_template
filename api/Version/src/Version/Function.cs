using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace Version
{
    public class VersionInfo
    {
        public string Version { get; set; }
    }
    public class Function
    {
        public VersionInfo FunctionHandler(string input, ILambdaContext context)
        {
            return new VersionInfo
            {
                Version = "0.1"
            };
        }
    }
}
