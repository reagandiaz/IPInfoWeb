using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using ReverseDNSService.Models;

namespace ReverseDNSService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WorkerController : ControllerBase
    {
        [HttpGet]
        public Result Get(string ip)
        {
            var result = new Result() { ip = ip };

            System.Net.IPAddress address;
            if (!System.Net.IPAddress.TryParse(ip, out address))
            {
                if (Uri.CheckHostName(ip) == UriHostNameType.Unknown)
                {
                    result.info = "Invalid ip/domain";
                    result.state = "Error";
                    result.ts = DateTime.Now;
                    return result;
                }
            }

            LoadResult(result);

            result.ts = DateTime.Now;
            return result;
        }

        void LoadResult(Result result)
        {
            try
            {
                System.Net.IPAddress address;
                if (System.Net.IPAddress.TryParse(result.ip, out address))
                {
                    //an ip
                    result.message = Dns.GetHostEntry(result.ip).HostName;
                    result.info = "Found";
                    result.state = "Complete";
                }
                else
                {
                    //domain
                    result.message = Dns.GetHostAddresses(result.ip)[0].ToString();
                    result.info = "Found";
                    result.state = "Complete";
                }
            }
            catch (System.Exception ex)
            {
                if (ex.Message.Contains("No such host is known") || ex.Message.Contains("Name or service not known"))
                {
                    result.message = "No such host is known";
                    result.info = "Name or service not known";
                    result.state = "Complete";
                }
                else
                {
                    result.message = ex.Message;
                    result.info = ex.StackTrace;
                    result.state = "Error";
                }
            }
        }
    }
}
