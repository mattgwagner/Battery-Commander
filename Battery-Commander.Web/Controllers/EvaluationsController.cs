﻿using BatteryCommander.Web.Models;
using BatteryCommander.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BatteryCommander.Web.Controllers
{
    [Authorize, ApiExplorerSettings(IgnoreApi = true)]
    public class EvaluationsController : Controller
    {
        private readonly Database db;

        private async Task<String> GetDisplayName()
        {
            var user = await UserService.FindAsync(db, User);

            return user?.ToString() ?? User.Identity.Name;
        }

        public EvaluationsController(Database db)
        {
            this.db = db;
        }

        public async Task<IActionResult> Index(Boolean includeComplete = false, Boolean onlyDelinquent = false)
        {
            return View("List", new EvaluationListViewModel
            {
                Evaluations = await Evaluations.Where(_ => !_.IsCompleted).ToListAsync(),
                Soldiers = await SoldierSearchService.Filter(db, new SoldierSearchService.Query { Ranks = RankExtensions.All().Where(_ => _.GetsEvaluation()) })
            });
        }

        public async Task<IActionResult> All()
        {
            return View("List", new EvaluationListViewModel
            {
                Evaluations = await Evaluations.ToListAsync(),
                Soldiers = await SoldierSearchService.Filter(db, new SoldierSearchService.Query { Ranks = RankExtensions.All().Where(_ => _.GetsEvaluation()) })
            });
        }

        public async Task<IActionResult> Details(int id)
        {
            return View(await Evaluations.SingleOrDefaultAsync(_ => _.Id == id));
        }

        public async Task<IActionResult> New(int soldier = 0)
        {
            ViewBag.Soldiers = await SoldiersController.GetDropDownList(db);

            if (soldier > 0)
            {
                var soldierModel = await SoldiersController.Get(db, soldier);

                var lastEval =
                    await db
                    .Evaluations
                    .Where(evaluation => evaluation.RateeId == soldier)
                    .OrderByDescending(evaluation => evaluation.ThruDate)
                    .FirstOrDefaultAsync();

                // Try to backtrack into their next evaluation period based on the last eval we have for them

                return View("Edit", new Evaluation
                {
                    RateeId = soldier,
                    RaterId = soldierModel.SupervisorId ?? 0,
                    SeniorRaterId = soldierModel.Supervisor?.SupervisorId ?? 0,
                    StartDate = lastEval?.ThruDate.AddDays(1) ?? DateTime.Today,
                    ThruDate = lastEval?.ThruDate.AddDays(1).AddYears(1) ?? DateTime.Today.AddYears(1)
                });
            }

            return View("Edit");
        }

        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.Soldiers = await SoldiersController.GetDropDownList(db);

            return View(await Evaluations.SingleOrDefaultAsync(_ => _.Id == id));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(Evaluation model)
        {
            if (await db.Evaluations.AnyAsync(evaluation => evaluation.Id == model.Id) == false)
            {
                model.Events.Add(new Evaluation.Event
                {
                    Author = await GetDisplayName(),
                    Message = "Added Evaluation"
                });

                db.Evaluations.Add(model);
            }
            else
            {
                model.Events.Add(new Evaluation.Event
                {
                    Author = await GetDisplayName(),
                    Message = "Evaluation Updated"
                });

                db.Evaluations.Update(model);
            }

            await db.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { model.Id });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Transition(int id, Evaluation.Trigger trigger)
        {
            var evaluation = await Evaluations.SingleOrDefaultAsync(_ => _.Id == id);

            evaluation.Transition(trigger);

            evaluation.Events.Add(new Evaluation.Event
            {
                Author = await GetDisplayName(),
                Message = trigger.DisplayName()
            });

            await db.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Comment(int id, String message)
        {
            var evaluation = await Evaluations.SingleOrDefaultAsync(_ => _.Id == id);

            evaluation.Events.Add(new Evaluation.Event
            {
                Author = await GetDisplayName(),
                Message = message
            });

            await db.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var evaluation = await Evaluations.SingleOrDefaultAsync(_ => _.Id == id);

            db.Evaluations.Remove(evaluation);

            await db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private IQueryable<Evaluation> Evaluations =>
                db
                .Evaluations
                .Include(_ => _.Ratee)
                .Include(_ => _.Rater)
                .Include(_ => _.SeniorRater)
                .Include(_ => _.Reviewer)
                .Include(_ => _.Events)
                .OrderBy(_ => _.ThruDate);
    }
}