using BatteryCommander.Web.Models;
using BatteryCommander.Web.Models.Reports;
using BatteryCommander.Web.Models.Settings;
using BatteryCommander.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BatteryCommander.Web.Controllers
{
    [Authorize, ApiExplorerSettings(IgnoreApi = true)]
    public class ReportsController : Controller
    {
        private readonly Database db;

        public ReportsController(Database db)
        {
            this.db = db;
        }

        [Route("~/Units/{unitId}/Reports", Name = "Unit.Reports")]
        public async Task<IActionResult> Index(int unitId)
        {
            return View(await UnitService.Get(db, unitId));
        }

        public async Task<IActionResult> Add(int unitId)
        {
            ViewBag.UnitId = unitId;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Toggle(int unitId, Report.ReportType type, Boolean enable)
        {
            var unit = await UnitService.Get(db, unitId);

            var settings =
                unit
                .ReportSettings
                .SingleOrDefault(s => s.Type == type);

            settings.Enabled = enable;

            return await Save(unitId, settings);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int unitId, Report.ReportType type)
        {
            var unit = await UnitService.Get(db, unitId);

            var new_list = unit.ReportSettings.ToList();

            new_list.RemoveAll(settings => settings.Type == type);

            unit.ReportSettings = new_list;

            await db.SaveChangesAsync();

            return RedirectToAction(nameof(Index), new { unitId });
        }

        public async Task<IActionResult> Save(int unitId, ReportSettings settings)
        {
            // Remove any if existing

            await Delete(unitId, settings.Type);

            var unit = await UnitService.Get(db, unitId);

            unit.ReportSettings =
                unit
                .ReportSettings
                .Union(new[] { settings })
                .ToList();

            await db.SaveChangesAsync();

            return RedirectToAction(nameof(Index), new { unitId });
        }

        // TAN 1 -- Comstat

        // YELLOW 1 -- LOGSTAT

        public async Task<IActionResult> SadPerstat(SoldierService.Query query)
        {
            var model = new StateActiveDuty_Perstat
            {
                Soldiers = await SoldierService.Filter(db, query)
            };

            return Json(model);
        }

        public async Task<IActionResult> DscaReady()
        {
            var soldiers = await SoldierService.Filter(db, new SoldierService.Query
            {
                IWQ = true,
                DSCA = true
            });

            return View(soldiers);
        }
    }
}