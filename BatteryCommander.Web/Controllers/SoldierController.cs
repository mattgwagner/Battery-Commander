using BatteryCommander.Common;
using BatteryCommander.Common.Models;
using BatteryCommander.Web.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI;

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

        [Route("~/Soldiers")]
        public async Task<ActionResult> List(int? groupId, int? rankId, int? positionId, int? mosId, Boolean activeOnly = true)
        {
            var soldiers =
                await _db
                .Soldiers
                .Where(s => !activeOnly || s.Status == SoldierStatus.Active)
                .Where(s => !groupId.HasValue || s.Group == (Group)groupId.Value)
                .Where(s => !rankId.HasValue || s.Rank == (Rank)rankId.Value)
                .Where(s => !mosId.HasValue || s.MOS == (MOS)mosId.Value)
                .Where(s => !positionId.HasValue || s.Position == (Position)positionId.Value)
                .OrderBy(s => s.LastName)
                .ThenBy(s => s.FirstName)
                .ToListAsync();

            return View(soldiers);
        }

        [Route("~/Soldier/{soldierId}")]
        public async Task<ActionResult> View(int soldierId)
        {
            var soldier =
                await _db
                .Soldiers
                .Include(s => s.Qualifications)
                .SingleOrDefaultAsync(s => s.Id == soldierId);

            return View(soldier);
        }

        [Route("~/Soldier/New")]
        public ActionResult New()
        {
            return View("Edit", new SoldierEditModel { });
        }

        [Route("~/Soldier/{soldierId}/Edit")]
        public async Task<ActionResult> Edit(int soldierId)
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
                model.SecurityClearance = soldier.SecurityClearance;
                model.MOS = soldier.MOS;
                model.Position = soldier.Position;
                model.IsDutyMOSQualified = soldier.IsDutyMOSQualified;
                model.EducationLevelCompleted = soldier.EducationLevelCompleted;
                model.Group = soldier.Group;
                model.ETSDate = soldier.ETSDate;
                model.Notes = soldier.Notes;
            }

            return View(model);
        }

        [Route("~/Soldiers/Bulk")]
        public async Task<ActionResult> Bulk()
        {
            var lines_to_add = 25;

            var soldiers =
                await _db
                .Soldiers
                .OrderBy(s => s.LastName)
                .ThenBy(s => s.FirstName)
                .Select(s => new SoldierEditModel
                {
                    Id = s.Id,
                    FirstName = s.FirstName,
                    LastName = s.LastName,
                    Rank = s.Rank,
                    Position = s.Position,
                    Status = s.Status,
                    SecurityClearance = s.SecurityClearance,
                    MOS = s.MOS,
                    IsDutyMOSQualified = s.IsDutyMOSQualified,
                    EducationLevelCompleted = s.EducationLevelCompleted,
                    Group = s.Group,
                    ETSDate = s.ETSDate,
                    Notes = s.Notes
                })
                .ToListAsync();

            soldiers.AddRange(Enumerable.Range(1, lines_to_add).Select(s => new SoldierEditModel { }).ToList());

            return View(soldiers);
        }

        [Route("~/Soldiers/Bulk")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Bulk(IEnumerable<SoldierEditModel> models)
        {
            foreach (var model in models)
            {
                if (String.IsNullOrWhiteSpace(model.FirstName) || String.IsNullOrWhiteSpace(model.LastName)) continue;

                await AddOrUpdate(model);
            }

            await _db.SaveChangesAsync();

            return RedirectToAction("List");
        }

        [Route("~/Soldier/Save")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(SoldierEditModel model)
        {
            if (!ModelState.IsValid) return View("Edit", model);

            var soldier = await AddOrUpdate(model);

            await _db.SaveChangesAsync();

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
            soldier.SecurityClearance = model.SecurityClearance;
            soldier.MOS = model.MOS;
            soldier.Position = model.Position;
            soldier.IsDutyMOSQualified = model.IsDutyMOSQualified;
            soldier.EducationLevelCompleted = model.EducationLevelCompleted;
            soldier.Group = model.Group;
            soldier.ETSDate = model.ETSDate;
            soldier.Notes = model.Notes;

            return soldier;
        }
    }
}