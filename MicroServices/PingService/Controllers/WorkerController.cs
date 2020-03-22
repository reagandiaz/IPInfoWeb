using System;
using System.Net.NetworkInformation;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using PingService.Models;

namespace PingService.Controllers
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
                string data = "Hello! This is Reagan Diaz";
                var pingSender = new System.Net.NetworkInformation.Ping();
                PingOptions options = new PingOptions
                {
                    DontFragment = true
                };
                byte[] buffer = Encoding.ASCII.GetBytes(data);
                const int timeout = 1024;
                PingReply reply = pingSender.Send(result.ip, timeout, buffer, options);
                result.message = new Message();
                if (reply.Status == IPStatus.Success)
                {
                    result.message.address = reply.Address.ToString();
                    result.message.roundtrip = reply.RoundtripTime;
                    result.message.ttl = reply.Options.Ttl;
                    result.message.dontfragment = reply.Options.DontFragment;
                    result.message.buffersize = reply.Buffer.Length;
                }
                result.info = Enum.GetName(reply.Status.GetType(), reply.Status);
                result.state = "Complete";
            }
            catch (Exception ex)
            {
                result.state = "Error";
                result.info = $"{ex.Message}:{ex.StackTrace}";
            }
        }

    }
}
