using BatteryCommander.Web.Models;
using BatteryCommander.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BatteryCommander.Web.Controllers
{
    public class ACFTController : Controller
    {
        private readonly Database db;

        public ACFTController(Database db)
        {
            this.db = db;
        }

        public async Task<IActionResult> Index(SoldierService.Query query)
        {
            return View("List", new ACFTListViewModel
            {
                Soldiers = await SoldierService.Filter(db, query)
            });
        }

        public async Task<IActionResult> Details(int id)
        {
            return View(await Get(db, id));
        }

        public async Task<IActionResult> New(int soldier = 0)
        {
            ViewBag.Soldiers = await SoldiersController.GetDropDownList(db, SoldierService.Query.ALL);

            return View(nameof(Edit), new ACFT { SoldierId = soldier });
        }

        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.Soldiers = await SoldiersController.GetDropDownList(db, SoldierService.Query.ALL);

            return View(await Get(db, id));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(ACFT model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Soldiers = await SoldiersController.GetDropDownList(db, SoldierService.Query.ALL);

                return View("Edit", model);
            }

            if (await db.ACFTs.AnyAsync(test => test.Id == model.Id) == false)
            {
                db.ACFTs.Add(model);
            }
            else
            {
                db.ACFTs.Update(model);
            }

            await db.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { model.Id });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var test = await Get(db, id);

            db.ACFTs.Remove(test);

            await db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public static async Task<ACFT> Get(Database db, int id)
        {
            return
                await db
                .ACFTs
                .Include(_ => _.Soldier)
                .ThenInclude(_ => _.Unit)
                .Where(_ => _.Id == id)
                .SingleOrDefaultAsync();
        }
    }
}
