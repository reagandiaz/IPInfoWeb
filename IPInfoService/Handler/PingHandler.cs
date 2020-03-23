using IPInfoService.Models;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace IPInfoService.Handler
{
    public class PingHandler : IHandler<object>
    {
        readonly string baseUrl;
        public PingHandler()
        {
            var config = Startup.OpenAPIConfig.config.SingleOrDefault(s => s.host == TaskName);
            baseUrl = config == null ? "http://localhost:3333" : config.url;
        }
        public string TaskName => "Ping";
        public ReportItem GenerateReport(Task task)
        {
            var tres = (Ping.Result)(((Task<object>)task).Result);
            var reportItem = new ReportItem() { task = TaskName };
            reportItem.data = tres.Message;
            reportItem.info = tres.Info;
            reportItem.state = tres.State;
            reportItem.offset = tres.Ts;
            return reportItem;
        }

        public async Task<object> TaskToRun(string ip)
        {
            Ping.Result result;
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    Ping.PingClient swclient = new Ping.PingClient(baseUrl, client);
                    result = await swclient.WorkerAsync(ip);
                }
                catch (Exception ex)
                {
                    result = new Ping.Result() { Ts = DateTime.Now, Info = $"{ex.Message}:{ex.Message}", State = "Error" };
                }
            }
            return (object)result;
        }
    }
}