﻿using BatteryCommander.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace BatteryCommander.Web.Controllers
{
    [Authorize]
    public class APFTController : Controller
    {
        private readonly Database db;

        public APFTController(Database db)
        {
            this.db = db;
        }

        public async Task<IActionResult> Index()
        {
            // TODO Filtering by pass/fail

            // TODO Group by last per-soldier?

            var tests =
                await db
                .APFTs
                .OrderByDescending(apft => apft.Date)
                .Include(apft => apft.Soldier)
                .ToListAsync();

            return View("List", tests);
        }

        public async Task<IActionResult> Details(int id)
        {
            return View(await Get(db, id));
        }

        public async Task<IActionResult> Counseling(int id)
        {
            var apft = await Get(db, id);

            return File(apft.GenerateCounseling(), "application/pdf");
        }

        public async Task<IActionResult> New()
        {
            ViewBag.Soldiers = await SoldiersController.GetDropDownList(db);

            return View(nameof(Edit), new APFT { });
        }

        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.Soldiers = await SoldiersController.GetDropDownList(db);

            return View(await Get(db, id));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(APFT model)
        {
            var apft = await Get(db, model.Id);

            if (apft == null)
            {
                db.APFTs.Add(model);
            }
            else
            {
                db.APFTs.Update(model);
            }

            await db.SaveChangesAsync();

            return RedirectToAction(nameof(Details), model.Id);
        }

        public static async Task<APFT> Get(Database db, int id)
        {
            return
                await db
                .APFTs
                .Include(_ => _.Soldier)
                .Include(_ => _.Soldier.Unit)
                .Where(_ => _.Id == id)
                .SingleOrDefaultAsync();
        }
    }
}