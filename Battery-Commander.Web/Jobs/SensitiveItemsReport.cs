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

        private readonly IFluentEmailFactory emailSvc;

        public SensitiveItemsReport(Database db, IFluentEmailFactory emailSvc)
        {
            this.db = db;
            this.emailSvc = emailSvc;
        }

        public virtual void Execute()
        {
            foreach (var unit in UnitService.List(db).GetAwaiter().GetResult())
            {
                if (unit.SensitiveItems.Enabled)
                {
                    emailSvc
                        .Create()
                        .To(unit.SensitiveItems.Recipients)
                        .SetFrom(unit.SensitiveItems.From.EmailAddress, unit.SensitiveItems.From.Name)
                        .Subject($"{unit.Name} | GREEN 3 Report | {unit.SensitiveItems.Status}")
                        .UsingTemplateFromFile($"{Directory.GetCurrentDirectory()}/Views/Reports/Green3_SensitiveItems.cshtml", unit)
                        .SendWithErrorCheck();
                }
            }
        }
    }
}