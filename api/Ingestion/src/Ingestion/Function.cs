using Amazon.Lambda.Core;
using Newtonsoft.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace Ingestion
{
    public class Function
    {
        
        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="message"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public void IngestMessage(Message message, ILambdaContext context)
        {
            context.Logger.LogLine("got message: " + JsonConvert.SerializeObject(message));
        }
    }

    public class Message
    {
        public string ClientId { get; set; }
        public string Timestamp { get; set; }
        public string MsgType { get; set; }
        public string MsgText { get; set; }
    }
}
