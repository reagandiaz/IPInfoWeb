namespace PingService.Models
{
    public class Message
    {
        public string address { get; set; }
        public long roundtrip { get; set; }
        public long ttl { get; set; }
        public bool dontfragment { get; set; }
        public int buffersize { get; set; }
    }
}
