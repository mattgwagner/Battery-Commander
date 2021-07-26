using BatteryCommander.Web.Models;
using BatteryCommander.Web.Queries;
using BatteryCommander.Web.Services;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BatteryCommander.Web.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class SoldiersController : Controller
    {
        private readonly Database db;
        private readonly IMediator dispatcher;

        public SoldiersController(Database db, IMediator dispatcher)
        {
            this.db = db;
            this.dispatcher = dispatcher;
        }

        [Route("~/Soldiers", Name = "Soldiers.List")]
        public async Task<IActionResult> Index(GetSoldiers query)
        {
            return View("List", new SoldierListViewModel
            {
                Query = query,
                Soldiers = await dispatcher.Send(query)
            });
        }

        [Route("~/Units/{unitId}/Soldiers", Name = "Unit.Soldiers")]
        public async Task<IActionResult> ForUnit(int unitId)
        {
            return await Index(new GetSoldiers { Unit = unitId });
        }

        [Route("~/Soldiers/All")]
        public async Task<IActionResult> All()
        {
            return View("List", new SoldierListViewModel
            {
                Soldiers = await dispatcher.Send(new GetSoldiers { })
            });
        }

        [Route("~/Soldiers/SignInRoster")]
        public async Task<IActionResult> SignInRoster(GetSoldiers query)
        {
            return View(new SoldierListViewModel
            {
                Query = query,
                Soldiers = await dispatcher.Send(query)
            });
        }

        [Route("~/Soldiers/{id}", Name = "Soldier.Details")]
        public async Task<IActionResult> Details(int id)
        {
            var soldier = await dispatcher.Send(new GetSoldier(id));

            if (soldier == null) return NotFound();

            var model = new SoldierDetailsViewModel
            {
                Soldier = soldier,
                Subordinates = await SoldierService.Subordinates(db, id),
                Evaluations =
                    db
                    .Evaluations
                    .Include(eval => eval.Ratee)
                    .Include(eval => eval.Rater)
                    .Include(eval => eval.SeniorRater)
                    .Include(eval => eval.Reviewer)
                    .AsEnumerable()
                    .Where(eval => new[] { eval.RateeId, eval.RaterId, eval.SeniorRaterId, eval.ReviewerId }.Any(sm => sm == id))
                    .OrderByDescending(eval => eval.ThruDate)
                    .Select(eval => new SoldierDetailsViewModel.EvaluationViewModel
                    {
                        Evaluation = eval,
                        Role = eval.RateeId == id ? "Soldier" : (eval.RaterId == id ? "Rater" : (eval.SeniorRaterId == id ? "Senior Rater" : "Review"))
                    })
                    .ToList()
            };

            return View(model);
        }

        [Route("~/Soldiers/New", Name = "Soldiers.New")]
        public async Task<IActionResult> New() => await Return_To_Edit(new Soldier { });

        public async Task<IActionResult> Edit(int id) => await Return_To_Edit(
                await db
                .Soldiers
                .Where(s => s.Id == id)
                .SingleAsync()
        );

        [Route("~/Soldiers"), HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(Soldier model, Boolean force = false)
        {
            if (!ModelState.IsValid)
            {
                return await Return_To_Edit(model);
            }

            if (await db.Soldiers.AnyAsync(soldier => soldier.Id == model.Id) == false)
            {
                if (!force)
                {
                    if (await db.Soldiers.AnyAsync(soldier => soldier.FirstName == model.FirstName && soldier.LastName == model.LastName))
                    {
                        ModelState.AddModelError("", "Potential duplicate Soldier being added, go to Soldiers->View All first");
                        return await Return_To_Edit(model);
                    }
                }

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
            await db.Database.ExecuteSqlInterpolatedAsync($@"
delete from ABCPs where SoldierId = {id};
delete from APFTs where SoldierId = {id};
delete from ACFTs where SoldierId = {id};
delete from Passengers where SoldierId = {id};
delete from SSDSnapshot where SoldierId = {id};

update Vehicles set DriverId = null where DriverId = {id};
update Vehicles set A_DriverId = null where DriverId = {id};
update Weapons set AssignedId = null where AssignedId = {id};

delete from Soldiers where Id = {id};
");

            return RedirectToAction(nameof(Index));
        }

        [Route("~/Units/{unitId}/Soldiers/Import")]
        public IActionResult Import(int unitId)
        {
            ViewBag.UnitId = unitId;

            return View();
        }

        [Route("~/Units/{unitId}/Soldiers/Import"), HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Import(int unitId, IFormFile file)
        {
            await BatchImportService.ImportSoldiers(db, file.OpenReadStream(), unitId);

            return RedirectToRoute("Unit.Soldiers", new { unitId });
        }

        [HttpPost, ValidateAntiForgeryToken, Route("~/Soldiers/SetStatus")]
        public async Task<IActionResult> SetStatus(int soldierId, Soldier.SoldierStatus status)
        {
            var soldier =
                await db
                .Soldiers
                .Where(s => s.Id == soldierId)
                .SingleAsync();

            soldier.Status = status;

            await db.SaveChangesAsync();

            return Redirect($"{Request.GetTypedHeaders().Referer}");
        }

        private async Task<IActionResult> Return_To_Edit(Soldier soldier)
        {
            ViewBag.Units = await UnitsController.GetDropDownList(db);
            ViewBag.Soldiers = await SoldierService.GetDropDownList(db);

            return View("Edit", soldier);
        }
    }
}