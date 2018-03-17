using BatteryCommander.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BatteryCommander.Web.Controllers
{
    [Authorize, ApiExplorerSettings(IgnoreApi = true)]
    public class WeaponsController : Controller
    {
        private readonly Database db;

        public WeaponsController(Database db)
        {
            this.db = db;
        }

        public async Task<IActionResult> Index()
        {
            // List of Weapons - by unit, by status

            return View("List", new WeaponListViewModel
            {
                Weapons =
                    await db
                    .Weapons
                    .Include(weapon => weapon.Unit)
                    .Include(weapon => weapon.Assigned)
                    .ToListAsync()
            });
        }

        public async Task<IActionResult> New()
        {
            ViewBag.Soldiers = await SoldiersController.GetDropDownList(db, includeIgnoredUnits: true);
            ViewBag.Units = await UnitsController.GetDropDownList(db);

            return View(nameof(Edit), new Weapon { });
        }

        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.Soldiers = await SoldiersController.GetDropDownList(db, includeIgnoredUnits: true);
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

            return File(weapon.GenerateReceipt(), "application/pdf", filename);
        }
    }
}