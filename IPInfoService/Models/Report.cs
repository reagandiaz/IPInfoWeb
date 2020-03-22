using System;

namespace IPInfoService.Models
{
    public class Report
    {
        public string ip { get; set; }
        public ReportItem[] reports { get; set; }
        public string message { get; set; }
        public DateTime start { get; set; }
        public DateTime end { get; set; }
    }
}
