using BatteryCommander.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
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

        public async Task<IActionResult> Index(int? unit = null)
        {
            // TODO Filtering by Rank, MOS, Position, Name, Status

            var soldiers =
                db
                .Soldiers
                .Include(_ => _.Unit)
                .Include(_ => _.ABCPs)
                .Include(_ => _.APFTs)
                .Where(_ => !unit.HasValue || _.UnitId == unit)
                .ToList() // SQLite case insentive search isn't available on EF?
                .OrderBy(_ => _.LastName)
                .ThenBy(_ => _.FirstName);

            return View("List", soldiers);
        }

        public async Task<IActionResult> Details(int id)
        {
            var model = new SoldierDetailsViewModel
            {
                Soldier = await Get(db, id),
                Evaluations =
                    await db
                    .Evaluations
                    .Include(eval => eval.Ratee)
                    .Include(eval => eval.Rater)
                    .Include(eval => eval.SeniorRater)
                    .Where(eval => new[] { eval.RateeId, eval.RaterId, eval.SeniorRaterId }.Any(sm => sm == id))
                    .Select(eval => new SoldierDetailsViewModel.EvaluationViewModel
                    {
                        Evaluation = eval,
                        Role = eval.RateeId == id ? "Soldier" : (eval.RaterId == id ? "Rater" : "Senior Rater")
                    })
                    .ToListAsync()
            };

            return View(model);
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
            if (await db.Soldiers.AnyAsync(soldier => soldier.Id == model.Id) == false)
            {
                db.Soldiers.Add(model);
            }
            else
            {
                db.Soldiers.Update(model);
            }

            await db.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { model.Id });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var soldier = await Get(db, id);

            db.Soldiers.Remove(soldier);

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

        public static async Task<IEnumerable<SelectListItem>> GetDropDownList(Database db, Boolean excludeEnlisted = false)
        {
            // TODO Filter by status, unit

            var soldiers =
                await db
                .Soldiers
                .Where(soldier => excludeEnlisted == false || soldier.IsNCO || soldier.IsOfficer)
                .OrderBy(soldier => soldier.LastName)
                .ThenBy(soldier => soldier.FirstName)
                .ToListAsync();

            return soldiers.Select(soldier => new SelectListItem
            {
                Text = $"{soldier}",
                Value = $"{soldier.Id}"
            });
        }
    }
}