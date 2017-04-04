using BatteryCommander.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BatteryCommander.Web.Controllers
{
    [Authorize]
    public class EvaluationsController : Controller
    {
        private readonly Database db;

        public EvaluationsController(Database db)
        {
            this.db = db;
        }

        public async Task<IActionResult> Index(Boolean includeComplete = false)
        {
            var evaluations =
                await db
                .Evaluations
                .Include(_ => _.Ratee)
                .Include(_ => _.Rater)
                .Include(_ => _.SeniorRater)
                .Include(_ => _.Events)
                .Where(_ => includeComplete || _.IsCompleted)
                .OrderBy(_ => _.ThruDate)
                .ToListAsync();

            return View("List", evaluations);
        }

        public async Task<IActionResult> Details(int id)
        {
            return View(await Get(db, id));
        }

        public async Task<IActionResult> New(int soldier = 0)
        {
            ViewBag.Soldiers = await SoldiersController.GetDropDownList(db);

            return View("Edit", new Evaluation { RateeId = soldier });
        }

        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.Soldiers = await SoldiersController.GetDropDownList(db);

            return View(await Get(db, id));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(Evaluation model)
        {
            var evaluation = await Get(db, model.Id);

            if (evaluation == null)
            {
                db.Evaluations.Add(model);
            }
            else
            {
                db.Evaluations.Update(model);
            }

            await db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<Evaluation> Get(Database db, int id)
        {
            return
                await db
                .Evaluations
                .Include(_ => _.Ratee)
                .Include(_ => _.Rater)
                .Include(_ => _.SeniorRater)
                .Include(_ => _.Events)
                .Where(_ => _.Id == id)
                .SingleOrDefaultAsync();
        }
    }
}