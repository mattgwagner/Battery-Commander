﻿using BatteryCommander.Web.Models;
using BatteryCommander.Web.Models.Reports;
using BatteryCommander.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        // Generate HTML/PDF version
        // Email to configurable address on request / on schedule

        // GREEN 3 -- Sensitive Items

        // CONVOY MANIFEST?

        // TAN 1 -- Comstat

        // YELLOW 1 -- LOGSTAT

        public async Task<IActionResult> Red1(SoldierService.Query query)
        {
            var model = new Red1_Perstat
            {
                Soldiers = await SoldierService.Filter(db, query)
            };

            return Json(model);
        }

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