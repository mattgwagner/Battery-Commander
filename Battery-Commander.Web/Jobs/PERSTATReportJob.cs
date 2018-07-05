using BatteryCommander.Web.Models;
using BatteryCommander.Web.Services;
using FluentEmail.Core;
using FluentScheduler;
using System.IO;

namespace BatteryCommander.Web.Jobs
{
    public class PERSTATReportJob : IJob
    {
        private readonly Database db;

        private readonly IFluentEmail emailSvc;

        public PERSTATReportJob(Database db, IFluentEmail emailSvc)
        {
            this.db = db;
            this.emailSvc = emailSvc;
        }

        public virtual void Execute()
        {
            var unit = UnitService.Get(db, unitId: 3).Result; // C Batt

            emailSvc
                .To("mattgwagner@gmail.com")
                .Subject("PERSTAT")
                .UsingTemplateFromFile($"{Directory.GetCurrentDirectory()}/Views/Reports/Red1_Perstat.cshtml", unit)
                .Send();
        }
    }
}