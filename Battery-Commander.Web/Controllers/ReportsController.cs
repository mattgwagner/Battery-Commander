using BatteryCommander.Web.Models;
using BatteryCommander.Web.Models.Reports;
using BatteryCommander.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BatteryCommander.Web.Controllers
{
    [Authorize]
    public class ReportsController : Controller
    {
        private readonly Database db;

        public ReportsController(Database db)
        {
            this.db = db;
        }

        // Generate HTML/PDF version
        // Email to configurable address on request / on schedule

        // GREEN 3 -- Sensitive Items

        // CONVOY MANIFEST?

        // TAN 1 -- Comstat

        // YELLOW 1 -- LOGSTAT

        public async Task<IActionResult> Red1(SoldierSearchService.Query query)
        {
            var model = new Red1_Perstat
            {
                Soldiers = await SoldierSearchService.Filter(db, query)
            };

            return Json(model);
        }

        public async Task<IActionResult> SadPerstat(SoldierSearchService.Query query)
        {
            var model = new StateActiveDuty_Perstat
            {
                Soldiers = await SoldierSearchService.Filter(db, query)
            };

            return Json(model);
        }

        public async Task<IActionResult> DscaReady()
        {
            var soldiers = await SoldierSearchService.Filter(db, new SoldierSearchService.Query
            {
                IWQ = true,
                DSCA = true
            });

            return View(soldiers);
        }
    }
}