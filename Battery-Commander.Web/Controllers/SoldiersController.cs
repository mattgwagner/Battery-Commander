using BatteryCommander.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BatteryCommander.Web.Controllers
{
    [Authorize]
    public class SoldiersController : Controller
    {
        private readonly Database db;

        public SoldiersController(Database db)
        {
            this.db = db;
        }

        public async Task<IActionResult> Index()
        {
            // TODO Filtering by Rank, MOS, Position, Name, Status

            var soldiers =
                await db
                .Soldiers
                .Include(_ => _.Unit)
                .OrderBy(_ => _.LastName)
                .ThenBy(_ => _.FirstName)
                .ToListAsync();

            return View("List", soldiers);
        }

        public async Task<IActionResult> Details(int id)
        {
            return Json(await Get(db, id));
        }

        public async Task<IActionResult> New()
        {
            ViewBag.Units = await UnitsController.GetDropDownList(db);

            return View("Edit", new Soldier { });
        }

        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.Units = await UnitsController.GetDropDownList(db);

            return View(await Get(db, id));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(Soldier model)
        {
            var soldier = await Get(db, model.Id);

            if (soldier == null)
            {
                db.Soldiers.Add(model);
            }
            else
            {
                db.Soldiers.Update(model);
            }

            await db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public static async Task<Soldier> Get(Database db, int id)
        {
            return
                await db
                .Soldiers
                .Include(_ => _.Unit)
                .Include(_ => _.APFTs)
                .Include(_ => _.ABCPs)
                .Where(_ => _.Id == id)
                .SingleOrDefaultAsync();
        }

        public static async Task<IEnumerable<SelectListItem>> GetDropDownList(Database db)
        {
            // TODO Filter by status, unit

            var soldiers =
                await db
                .Soldiers
                .OrderBy(soldier => soldier.LastName)
                .ThenBy(soldier => soldier.FirstName)
                .ToListAsync();

            return soldiers.Select(soldier => new SelectListItem
            {
                Text = $"{soldier.Rank.ShortName()} {soldier.LastName}, {soldier.FirstName}",
                Value = $"{soldier.Id}"
            });
        }
    }
}