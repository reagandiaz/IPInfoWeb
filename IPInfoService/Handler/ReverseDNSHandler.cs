using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using IPInfoService.Models;

namespace IPInfoService.Handler
{
    public class ReverseDNSHandler : IHandler<object>
    {
        readonly string baseUrl;
        public ReverseDNSHandler()
        {
            var config = Startup.OpenAPIConfig.config.SingleOrDefault(s => s.host == TaskName);
            baseUrl = config == null ? "http://localhost:4444" : config.url;
        }
        public string TaskName => "ReverseDNS";
        public ReportItem GenerateReport(Task task)
        {
            var tres = (ReverseDNS.Result)(((Task<object>)task).Result);
            var reportItem = new ReportItem() { task = TaskName };
            reportItem.data = tres.Message;
            reportItem.info = tres.Info;
            reportItem.state = tres.State;
            reportItem.offset = tres.Ts;
            return reportItem;
        }

        public async Task<object> TaskToRun(string ip)
        {
            ReverseDNS.Result result;
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    ReverseDNS.ReverseDNSClient swclient = new ReverseDNS.ReverseDNSClient(baseUrl, client);
                    result = await swclient.WorkerAsync(ip);
                }
                catch (Exception ex)
                {
                    result = new ReverseDNS.Result() { Ts = DateTime.Now, Info = $"{ex.Message}:{ex.StackTrace}", State = "Error" };
                }
            }
            return (object)result;
        }
    }
}