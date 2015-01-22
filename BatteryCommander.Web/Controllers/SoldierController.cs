using BatteryCommander.Common;
using BatteryCommander.Common.Models;
using BatteryCommander.Web.Models;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BatteryCommander.Web.Controllers
{
    public class SoldierController : BaseController
    {
        private readonly DataContext _db;

        public SoldierController(UserManager<AppUser, int> userManager, DataContext db)
            : base(userManager)
        {
            _db = db;
        }

        [Route("Soldiers")]
        public async Task<ActionResult> List()
        {
            var soldiers =
                await _db
                .Soldiers
                .ToListAsync();

            return View(soldiers);
        }

        [Route("Soldier/{soldierId}")]
        public async Task<ActionResult> View(int soldierId)
        {
            var soldier =
                await _db
                .Soldiers
                .Include(s => s.Qualifications)
                .SingleOrDefaultAsync(s => s.Id == soldierId);

            return View(soldier);
        }

        [Route("Soldier/{soldierId}/Edit")]
        [Route("Soldier/New")]
        public async Task<ActionResult> Edit(int? soldierId)
        {
            var model = new SoldierEditModel { };

            var soldier =
                await _db
                .Soldiers
                .SingleOrDefaultAsync(s => s.Id == soldierId);

            if (soldier != null)
            {
                model.Id = soldier.Id;
                model.FirstName = soldier.FirstName;
                model.LastName = soldier.LastName;
                model.Rank = soldier.Rank;
                model.Status = soldier.Status;
            }

            return View(model);
        }

        [Route("Soldiers/Bulk")]
        public async Task<ActionResult> Bulk()
        {
            var soldiers =
                await _db
                .Soldiers
                .Select(s => new SoldierEditModel
                {
                    Id = s.Id,
                    FirstName = s.FirstName,
                    LastName = s.LastName,
                    Rank = s.Rank,
                    Status = s.Status
                })
                .ToListAsync();

            return View(soldiers);
        }

        [Route("Soldiers/Bulk")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Bulk(IEnumerable<SoldierEditModel> models)
        {
            foreach (var model in models)
            {
                await AddOrUpdate(model);
            }

            return RedirectToAction("List");
        }

        [Route("Soldier")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(SoldierEditModel model)
        {
            if (!ModelState.IsValid) return View("Edit", model);

            var soldier = await AddOrUpdate(model);

            return RedirectToAction("View", new { soldierId = soldier.Id });
        }

        private async Task<Soldier> AddOrUpdate(SoldierEditModel model)
        {
            var soldier =
                await _db
                .Soldiers
                .SingleOrDefaultAsync(s => s.Id == model.Id);

            // TODO Check for duplicates by first/last name?

            if (soldier == null)
            {
                soldier = _db.Soldiers.Add(new Soldier { });
            }

            soldier.FirstName = model.FirstName;
            soldier.LastName = model.LastName;
            soldier.Rank = model.Rank;
            soldier.Status = model.Status;

            await _db.SaveChangesAsync();

            return soldier;
        }
    }
}