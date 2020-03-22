using IPInfoService.Models;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace IPInfoService.Handler
{
    public class PingHandler : IHandler<object>
    {
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
                    string baseUrl = "http://localhost:3333";
                    Ping.swagger1Client swclient = new Ping.swagger1Client(baseUrl, client);
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