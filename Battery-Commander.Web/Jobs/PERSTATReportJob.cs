using BatteryCommander.Web.Models;
using BatteryCommander.Web.Services;
using FluentEmail.Core;
using FluentScheduler;
using Serilog;
using System.IO;

namespace BatteryCommander.Web.Jobs
{
    public class PERSTATReportJob : IJob
    {
        private static readonly ILogger Log = Serilog.Log.ForContext<PERSTATReportJob>();

        private readonly Database db;

        private readonly IFluentEmailFactory emailSvc;

        public PERSTATReportJob(Database db, IFluentEmailFactory emailSvc)
        {
            this.db = db;
            this.emailSvc = emailSvc;
        }

        public virtual void Execute()
        {
            foreach (var unit in UnitService.List(db).GetAwaiter().GetResult())
            {
                if (unit.PERSTAT.Enabled)
                {
                    emailSvc
                        .Create()
                        .To(unit.PERSTAT.Recipients)
                        .SetFrom(unit.PERSTAT.From.EmailAddress, unit.PERSTAT.From.Name)
                        .Subject($"{unit.Name} | RED 1 PERSTAT")
                        .UsingTemplateFromFile($"{Directory.GetCurrentDirectory()}/Jobs/Red1_Perstat.html", unit)
                        .SendWithErrorCheck()
                        .Wait();
                }
            }
        }
    }
}