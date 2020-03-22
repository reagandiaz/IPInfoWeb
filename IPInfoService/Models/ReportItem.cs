using System;

namespace IPInfoService.Models
{
    public class ReportItem
    {
        public string task { get; set; }
        public object data { get; set; }
        public string state { get; set; }
        public string info { get; set; }
        public DateTimeOffset offset { get; set; }
    }
}