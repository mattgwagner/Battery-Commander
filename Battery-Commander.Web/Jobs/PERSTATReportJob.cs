using BatteryCommander.Web.Models;
using BatteryCommander.Web.Services;
using FluentScheduler;
using Serilog;

namespace BatteryCommander.Web.Jobs
{
    public class PERSTATReportJob : IJob
    {
        private static readonly ILogger Log = Serilog.Log.ForContext<PERSTATReportJob>();

        private readonly Database db;
        private readonly ReportService reportService;

        public PERSTATReportJob(Database db, ReportService reportService)
        {
            this.db = db;
            this.reportService = reportService;
        }

        public virtual void Execute()
        {
            foreach (var unit in UnitService.List(db).GetAwaiter().GetResult())
            {
                if (unit.PERSTAT.Enabled)
                {
                    reportService
                        .SendPerstatReport(unit.Id)
                        .Wait();
                }
            }
        }
    }
}