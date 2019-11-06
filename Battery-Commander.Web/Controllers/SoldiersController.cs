using BatteryCommander.Web.Models;
using BatteryCommander.Web.Services;
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
    [ApiExplorerSettings(IgnoreApi = true)]
    public class SoldiersController : Controller
    {
        private readonly Database db;

        public SoldiersController(Database db)
        {
            this.db = db;
        }

        [Route("~/Soldiers", Name = "Soldiers.List")]
        public async Task<IActionResult> Index(SoldierService.Query query)
        {
            return View("List", new SoldierListViewModel
            {
                Query = query,
                Soldiers = await SoldierService.Filter(db, query)
            });
        }

        [Route("~/Units/{unitId}/Soldiers", Name = "Unit.Soldiers")]
        public async Task<IActionResult> ForUnit(int unitId)
        {
            return await Index(new SoldierService.Query { Unit = unitId });
        }

        [Route("~/Soldiers/All")]
        public async Task<IActionResult> All()
        {
            return View("List", new SoldierListViewModel
            {
                Query = SoldierService.Query.ALL,
                Soldiers = await SoldierService.Filter(db, SoldierService.Query.ALL)
            });
        }

        [Route("~/Soldiers/SignInRoster")]
        public async Task<IActionResult> SignInRoster(SoldierService.Query query)
        {
            return View(new SoldierListViewModel
            {
                Query = query,
                Soldiers = await SoldierService.Filter(db, query)
            });
        }

        [Route("~/Soldiers/{id}", Name = "Soldier.Details")]
        public async Task<IActionResult> Details(int id)
        {
            var model = new SoldierDetailsViewModel
            {
                Soldier = await Get(db, id),
                Subordinates = await SoldierService.Subordinates(db, id),
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
        public async Task<IActionResult> New() => await Return_To_Edit(new Soldier { });

        public async Task<IActionResult> Edit(int id) => await Return_To_Edit(await Get(db, id));

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
            await db.Database.ExecuteSqlCommandAsync($@"
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

        [Route("~/Soldiers/Import")]
        public IActionResult Import()
        {
            return View();
        }

        [Route("~/Soldiers/Import"), HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Import(IFormFile file)
        {
            await BatchImportService.ImportSoldiers(db, file.OpenReadStream());

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
            var soldiers = await SoldierService.Filter(db, new SoldierService.Query { Id = id });

            return soldiers.SingleOrDefault();
        }

        public static async Task<IEnumerable<SelectListItem>> GetDropDownList(Database db, SoldierService.Query query)
        {
            var soldiers = await SoldierService.Filter(db, query);

            return soldiers.Select(soldier => new SelectListItem
            {
                Text = $"{soldier}",
                Value = $"{soldier.Id}"
            });
        }

        [Obsolete]
        public static async Task<IEnumerable<SelectListItem>> GetDropDownList(Database db, Boolean includeIgnoredUnits = true)
        {
            return await GetDropDownList(db, SoldierService.Query.ALL);
        }

        private async Task<IActionResult> Return_To_Edit(Soldier soldier)
        {
            ViewBag.Units = await UnitsController.GetDropDownList(db);
            ViewBag.Soldiers = await SoldiersController.GetDropDownList(db);

            return View("Edit", soldier);
        }
    }
}