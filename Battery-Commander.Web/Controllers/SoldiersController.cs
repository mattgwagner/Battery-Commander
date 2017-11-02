using BatteryCommander.Web.Models;
using BatteryCommander.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BatteryCommander.Web.Controllers
{
    [Authorize, ApiExplorerSettings(IgnoreApi = true)]
    public class SoldiersController : Controller
    {
        private readonly Database db;

        public SoldiersController(Database db)
        {
            this.db = db;
        }


        [Route("~/Soldiers", Name = "Soldiers.List")]
        public async Task<IActionResult> Index(SoldierSearchService.Query query)
        {
            return View("List", new SoldierListViewModel
            {
                Soldiers = await SoldierSearchService.Filter(db, query)
            });
        }

        [Route("~/Soldiers/All")]
        public async Task<IActionResult> All()
        {
            return View("List", new SoldierListViewModel
            {
                Soldiers = await SoldierSearchService.Filter(db, new SoldierSearchService.Query
                {
                    IncludeIgnoredUnits = true
                })
            });
        }

        [Route("~/Soldiers/{id}", Name = "Soldier.Details")]
        public async Task<IActionResult> Details(int id)
        {
            var model = new SoldierDetailsViewModel
            {
                Soldier = await Get(db, id),
                Subordinates =
                    await db
                    .Soldiers
                    .Where(s => s.SupervisorId == id)
                    .ToListAsync(),
                Evaluations =
                    await db
                    .Evaluations
                    .Include(eval => eval.Ratee)
                    .Include(eval => eval.Rater)
                    .Include(eval => eval.SeniorRater)
                    .Include(eval => eval.Reviewer)
                    .Where(eval => new[] { eval.RateeId, eval.RaterId, eval.SeniorRaterId, eval.ReviewerId }.Any(sm => sm == id))
                    .OrderByDescending(eval => eval.ThruDate)
                    .Select(eval => new SoldierDetailsViewModel.EvaluationViewModel
                    {
                        Evaluation = eval,
                        Role = eval.RateeId == id ? "Soldier" : (eval.RaterId == id ? "Rater" : (eval.SeniorRaterId == id ? "Senior Rater" : "Review"))
                    })
                    .ToListAsync()
            };

            return View(model);
        }

        [Route("~/Soldiers/New", Name = "Soldiers.New")]
        public async Task<IActionResult> New()
        {
            ViewBag.Units = await UnitsController.GetDropDownList(db);
            ViewBag.Soldiers = await SoldiersController.GetDropDownList(db);

            return View("Edit", new Soldier { });
        }

        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.Units = await UnitsController.GetDropDownList(db);
            ViewBag.Soldiers = await SoldiersController.GetDropDownList(db);

            return View(await Get(db, id));
        }

        [Route("~/Soldiers"), HttpPost, ValidateAntiForgeryToken]
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

        public IActionResult Import()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Import(IFormFile file)
        {
            await DTMSService.ImportSoldiers(db, file.OpenReadStream());

            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken, Route("~/Soldiers/SetStatus")]
        public async Task<IActionResult> SetStatus(int soldierId, Soldier.SoldierStatus status)
        {
            var soldier = await Get(db, soldierId);

            soldier.Status = status;

            await db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public static async Task<Soldier> Get(Database db, int id)
        {
            var soldiers = await SoldierSearchService.Filter(db, new SoldierSearchService.Query { Id = id, IncludeIgnoredUnits = true });

            return soldiers.SingleOrDefault();
        }

        public static async Task<IEnumerable<SelectListItem>> GetDropDownList(Database db, Boolean includeIgnoredUnits = true)
        {
            var soldiers = await SoldierSearchService.Filter(db, new SoldierSearchService.Query
            {
                IncludeIgnoredUnits = includeIgnoredUnits
            });

            return soldiers.Select(soldier => new SelectListItem
            {
                Text = $"{soldier}",
                Value = $"{soldier.Id}"
            });
        }
    }
}