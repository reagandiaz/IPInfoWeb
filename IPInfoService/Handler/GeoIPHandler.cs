using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using IPInfoService.Models;

namespace IPInfoService.Handler
{
    public class GeoIPHandler : IHandler<object>
    {
        readonly string baseUrl;
        public GeoIPHandler()
        {
            var config = Startup.OpenAPIConfig.config.SingleOrDefault(s => s.host == TaskName);
            baseUrl = config == null ? "http://localhost:2222" : config.url;
        }
        public string TaskName => "GeoIP";
        public ReportItem GenerateReport(Task task)
        {
            var tres = (GeoIP.Result)(((Task<object>)task).Result);
            var reportItem = new ReportItem() { task = TaskName };
            reportItem.data = tres.Message;
            reportItem.info = tres.Info;
            reportItem.state = tres.State;
            reportItem.offset = tres.Ts;
            return reportItem;
        }
        public async Task<object> TaskToRun(string ip)
        {
            GeoIP.Result result;
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    GeoIP.GeoIPClient swclient = new GeoIP.GeoIPClient(baseUrl, client);
                    result = await swclient.WorkerAsync(ip);
                }
                catch (Exception ex)
                {
                    result = new GeoIP.Result() { Ts = DateTime.Now, Info = $"{ex.Message}:{ex.StackTrace}", State = "Error" };
                }
            }
            return (object)result;
        }
    }
}