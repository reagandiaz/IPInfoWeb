using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace IPInfoService.Models
{
    public class Request
    {
        [Required]
        [DefaultValue("yahoo.com")]
        public string ip { get; set; }

        [Required]
        [DefaultValue(new string[] { "GeoIP", "Ping", "ReverseDNS" })]
        public string[] tasks { get; set; }
    }
}
