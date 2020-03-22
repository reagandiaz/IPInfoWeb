using IPInfoService.Models;
using System.Threading.Tasks;

namespace IPInfoService.Handler
{
    public interface IHandler<T>
    {
        public string TaskName { get; }
        public Task<T> TaskToRun(string ip);
        public ReportItem GenerateReport(Task task);
    }
}