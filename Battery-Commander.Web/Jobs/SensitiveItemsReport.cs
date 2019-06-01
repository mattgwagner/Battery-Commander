using BatteryCommander.Web.Models;
using BatteryCommander.Web.Services;
using FluentEmail.Core;
using FluentScheduler;
using Serilog;
using System.IO;

namespace BatteryCommander.Web.Jobs
{
    public class SensitiveItemsReport : IJob
    {
        private static readonly ILogger Log = Serilog.Log.ForContext<SensitiveItemsReport>();

        private readonly Database db;

        private readonly ReportService reportService;

        public SensitiveItemsReport(Database db, ReportService reportService)
        {
            this.db = db;
            this.reportService = reportService;
        }

        public virtual void Execute()
        {
            foreach (var unit in UnitService.List(db).GetAwaiter().GetResult())
            {
                if (unit.SensitiveItems.Enabled)
                {
                    reportService
                        .SendPerstatReport(unit.Id)
                        .Wait();
                }
            }
        }
    }
}