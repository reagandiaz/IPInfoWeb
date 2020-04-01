using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using IPInfoService.Models;
using System.Linq;
using IPInfoService.Handler;


namespace IPInfoService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReportController : Controller
    {
        static List<Handler.IHandler<object>> ActiveHandlers = new List<IHandler<object>>() { new GeoIPHandler(), new PingHandler(), new ReverseDNSHandler() };

        [HttpPost]
        public Report Get(Request req)
        {
            Report report = new Report() { ip = req.ip, start = DateTime.Now };

            if (req.ip == null)
            {
                report.message = "Error:ip can't be null";
                return report;
            }

            System.Net.IPAddress address;
            if (!System.Net.IPAddress.TryParse(req.ip, out address))
            {
                if (Uri.CheckHostName(req.ip) == UriHostNameType.Unknown)
                {
                    report.message = "Error:incorrect ip format";
                    return report;
                }
            }

            var thandler = ActiveHandlers.Where(s => req.tasks.Contains(s.TaskName)).ToList();
            var tasks = new List<Task>();
            thandler.ForEach(s => { tasks.Add(Task.Run(() => s.TaskToRun(req.ip))); });

            try
            {
                Task.WaitAll(tasks.ToArray());
                report.reports = new ReportItem[tasks.Count];
                for (int i = 0; i < tasks.Count; i++)
                    report.reports[i] = thandler[i].GenerateReport(tasks[i]);
            }
            catch (Exception ex)
            {
                report.message = $"Error:{ex.Message}";
                Debug.WriteLine(ex.Message);
            }

            report.message = "Complete";
            report.end = DateTime.Now;
            return report;
        }
    }
}
