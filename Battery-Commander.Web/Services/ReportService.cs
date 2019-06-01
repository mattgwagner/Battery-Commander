using BatteryCommander.Web.Models;
using FluentEmail.Core;
using System.IO;
using System.Threading.Tasks;

namespace BatteryCommander.Web.Services
{
    public class ReportService
    {
        private readonly Database db;
        private readonly IFluentEmailFactory emailSvc;

        public ReportService(Database db, IFluentEmailFactory emailSvc)
        {
            this.db = db;
            this.emailSvc = emailSvc;
        }

        public async Task SendPerstatReport(int unitId)
        {
            var unit = await UnitService.Get(db, unitId);

            await emailSvc
                .Create()
                .To(unit.PERSTAT.Recipients)
                .SetFrom(unit.PERSTAT.From.EmailAddress, unit.PERSTAT.From.Name)
                .Subject($"{unit.Name} | RED 1 PERSTAT")
                .UsingTemplateFromFile($"{Directory.GetCurrentDirectory()}/Jobs/Red1_Perstat.html", unit)
                .SendWithErrorCheck();
        }
    }
}