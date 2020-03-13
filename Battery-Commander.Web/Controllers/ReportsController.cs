using BatteryCommander.Web.Models;
using BatteryCommander.Web.Models.Reports;
using BatteryCommander.Web.Models.Settings;
using BatteryCommander.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BatteryCommander.Web.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ReportsController : Controller
    {
        private readonly Database db;
        private readonly ReportService reportService;

        public ReportsController(Database db, ReportService reportService)
        {
            this.db = db;
            this.reportService = reportService;
        }

        [Route("~/Units/{unitId}/Reports", Name = "Unit.Reports")]
        public async Task<IActionResult> Index(int unitId)
        {
            ViewBag.UnitId = unitId;

            return View(await UnitService.Get(db, unitId));
        }

        public async Task<IActionResult> Add(int unitId)
        {
            ViewBag.UnitId = unitId;

            var settings = new ReportSettings
            {
                Enabled = true
            };

            settings.Recipients.Add(new FluentEmail.Core.Models.Address { });

            return View(settings);
        }

        [HttpPost]
        public async Task<IActionResult> SendPerstat(int unitId)
        {
            await reportService.SendPerstatReport(unitId);

            return RedirectToRoute("Unit.Details", new { id = unitId });
        }

        [HttpPost]
        public async Task<IActionResult> SendSensitiveItems(int unitId)
        {
            await reportService.SendSensitiveItems(unitId);

            return RedirectToRoute("Unit.Details", new { id = unitId });
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

        [HttpPost]
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

        // List status for all soldiers by - filter by unit

        // Bulk update statuses

        [HttpGet("~/Units/{Unit}/PERSTAT")]
        public async Task<IActionResult> PERSTAT(int Unit)
        {
            return View(new StatusListModel
            {
                Unit = Unit,
                Rows =
                    (await SoldierService.Filter(db, new SoldierService.Query
                    {
                        Unit = Unit
                    }))
                    .Select(soldier => new StatusListModel.Row
                    {
                        Soldier = soldier,
                        SoldierId = soldier.Id,
                        Status = soldier.Status
                    })
                    .ToList()
            });
        }

        [HttpPost("~/Unis/{Unit}/PERSTAT"), ValidateAntiForgeryToken]
        public async Task<IActionResult> PERSTAT(int Unit, StatusListModel model)
        {
            // For each DTO posted, update the soldier info

            foreach (var dto in model.Rows)
            {
                var soldier =
                    await db
                    .Soldiers
                    .Where(_ => _.Id == dto.SoldierId)
                    .SingleOrDefaultAsync();

                soldier.Status = dto.Status;
            }

            await db.SaveChangesAsync();

            return RedirectToAction(nameof(SoldiersController.ForUnit), "Soldiers", new { unitId = Unit });
        }

        public class StatusListModel
        {
            public IList<Row> Rows { get; set; }

            public int Unit { get; set; }

            public class Row
            {
                public Soldier Soldier { get; set; }

                public int SoldierId { get; set; }

                public Soldier.SoldierStatus Status { get; set; } = Soldier.SoldierStatus.Unknown;
            }
        }
    }
}
