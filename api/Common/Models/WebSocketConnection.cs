using Amazon.DynamoDBv2.DataModel;

namespace Common.Models
{
    public class WebSocketConnection
    {
        [DynamoDBHashKey]
        public string Connected { get; set; }
        public string ConnectionId { get; set; }
    }
}