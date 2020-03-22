using System;
namespace PingService.Models
{
    public class Result
    {
        public string ip { get; set; }
        public Message message { get; set; }
        public string state { get; set; }
        public string info { get; set; }
        public DateTime ts { get; set; }
    }
}
