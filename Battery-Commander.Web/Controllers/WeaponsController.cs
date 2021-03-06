﻿using BatteryCommander.Web.Models;
using BatteryCommander.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BatteryCommander.Web.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class WeaponsController : Controller
    {
        private readonly Database db;

        public WeaponsController(Database db)
        {
            this.db = db;
        }

        public async Task<IActionResult> Index(int? unit)
        {
            // List of Weapons - by unit, by status

            return View("List", new WeaponListViewModel
            {
                Weapons =
                    await db
                    .Weapons
                    .Include(weapon => weapon.Unit)
                    .Include(weapon => weapon.Assigned)
                    .Where(weapon => !unit.HasValue || weapon.UnitId == unit)
                    .ToListAsync()
            });
        }

        public async Task<IActionResult> New()
        {
            ViewBag.Soldiers = await SoldierService.GetDropDownList(db);
            ViewBag.Units = await UnitsController.GetDropDownList(db);

            return View(nameof(Edit), new Weapon { });
        }

        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.Soldiers = await SoldierService.GetDropDownList(db);
            ViewBag.Units = await UnitsController.GetDropDownList(db);

            var model =
                await db
                .Weapons
                .Where(_ => _.Id == id)
                .SingleOrDefaultAsync();

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(Weapon model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Soldiers = await SoldierService.GetDropDownList(db);
                ViewBag.Units = await UnitsController.GetDropDownList(db);
                return View(nameof(Edit), model);
            }

            if (await db.Weapons.AnyAsync(weapon => weapon.Id == model.Id) == false)
            {
                db.Weapons.Add(model);
            }
            else
            {
                db.Weapons.Update(model);
            }

            await db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var weapon = await db.Weapons.SingleAsync(_ => _.Id == id);

            db.Weapons.Remove(weapon);

            await db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Receipt(int id)
        {
            var weapon =
                await db
                .Weapons
                .Include(_ => _.Assigned)
                .Include(_ => _.Unit)
                .SingleAsync(_ => _.Id == id);

            var filename = $"{weapon.Unit.Name}_DA3749_{weapon.Assigned?.LastName}_{DateTime.Today:yyyyMMdd}.pdf";

            return File(await weapon.GenerateReceipt(), "application/pdf", filename);
        }
    }
}