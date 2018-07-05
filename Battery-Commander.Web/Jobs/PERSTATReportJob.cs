using BatteryCommander.Web.Models;
using BatteryCommander.Web.Services;
using FluentEmail.Core;
using FluentEmail.Core.Models;
using FluentScheduler;
using System.Collections.Generic;
using System.IO;

namespace BatteryCommander.Web.Jobs
{
    public class PERSTATReportJob : IJob
    {
        private static IList<Address> CC = new List<Address>(new[]
        {
            new Address { Name = "Matt Wagner", EmailAddress = "MattGWagner@Gmail.com" }
        });

        private static IList<Address> Recipients = new List<Address>(new Address[]
        {
            // new Address { Name = "2-116 FA BN TOC", EmailAddress = "ng.fl.flarng.list.ngfl-2-116-fa-bn-toc@mail.mil" }
        });

        private readonly Database db;

        private readonly IFluentEmail emailSvc;

        public PERSTATReportJob(Database db, IFluentEmail emailSvc)
        {
            this.db = db;
            this.emailSvc = emailSvc;
        }

        public virtual void Execute()
        {
            // HACK - Configure the recipients and units that this is going to be wired up for

            var unit = UnitService.Get(db, unitId: 3).Result; // C Batt

            emailSvc
                .To(CC)
                .CC(CC)
                .Subject($"RED 1 PERSTAT")
                .UsingTemplateFromFile($"{Directory.GetCurrentDirectory()}/Views/Reports/Red1_Perstat.cshtml", unit)
                .Send();
        }
    }
}