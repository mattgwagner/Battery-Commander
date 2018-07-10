using BatteryCommander.Web.Models;
using BatteryCommander.Web.Services;
using FluentEmail.Core;
using FluentEmail.Core.Models;
using FluentScheduler;
using System.IO;
using System.Linq;

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
            foreach (var unit in UnitService.List(db, includeIgnored: true).GetAwaiter().GetResult())
            {
                if (unit.Configuration.SendPERSTAT)
                {
                    var recipients =
                        unit
                        .Configuration
                        .PERSTAT_Recipients
                        .Select(address => new Address
                        {
                            EmailAddress = address
                        })
                        .ToList();

                    var from_address = unit.Configuration.PERSTAT_From;

                    emailSvc
                        .To(recipients)
                        .SetFrom(from_address)
                        .Subject($"{unit.Name} | RED 1 PERSTAT")
                        .UsingTemplateFromFile($"{Directory.GetCurrentDirectory()}/Views/Reports/Red1_Perstat.cshtml", unit)
                        .Send();
                }
            }
        }
    }
}