namespace Ingestion.Models
{
    public class Message
    {
        public string ClientId { get; set; }
        public string Timestamp { get; set; }
        public string MsgType { get; set; }
        public string MsgText { get; set; }
    }
}