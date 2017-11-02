﻿using BatteryCommander.Web.Models;
using BatteryCommander.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace BatteryCommander.Web.Controllers
{
    [Authorize, ApiExplorerSettings(IgnoreApi = true)]
    public class SSDController : Controller
    {
        private readonly Database db;

        public SSDController(Database db)
        {
            this.db = db;
        }

        public async Task<IActionResult> Index(SoldierSearchService.Query query)
        {
            // Ensure we're only displaying Soldiers we care about here

            if (!query.Ranks.Any()) query.OnlyEnlisted = true;

            return View("List", await SoldierSearchService.Filter(db, query));
        }

        [HttpPost]
        public async Task<IActionResult> Update(int soldierId, SSD ssd, decimal completion)
        {
            // Take the models and pull the updated data

            var soldier = await SoldiersController.Get(db, soldierId);

            soldier
                .SSDSnapshots
                .Add(new Soldier.SSDSnapshot
                {
                    SSD = ssd,
                    PerecentComplete = completion / 100 // Convert to decimal percentage
                });

            await db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}