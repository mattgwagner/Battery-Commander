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

namespace BatteryCommander.Web.Controllers
{
    public class SoldierQualificationController : BaseController
    {
        private readonly DataContext _db;

        public SoldierQualificationController(UserManager<AppUser, int> userManager, DataContext db)
            : base(userManager)
        {
            _db = db;
        }

        [Route("Soldier/{soldierId}/Qualification/{qualificationId}")]
        public async Task<ActionResult> Edit(int soldierId, int qualificationId)
        {
            var model = new SoldierQualificationEditModel
            {
                SoldierId = soldierId,
                QualificationId = qualificationId,
                PossibleQualifications = await GetQuals()
            };

            var qual =
                await _db
                .SoldierQualifications
                .Include(s => s.Soldier)
                .Include(q => q.Qualification)
                .Where(s => s.SoldierId == soldierId)
                .Where(q => q.QualificationId == qualificationId)
                .SingleOrDefaultAsync();

            if (qual != null)
            {
                model.Status = qual.Status;
                model.QualificationDate = qual.QualificationDate;
                model.ExpirationDate = qual.ExpirationDate;
            }

            return View(model);
        }

        [Route("Soldier/{soldierId}/Qualification/New")]
        public async Task<ActionResult> New(int soldierId)
        {
            var model = new SoldierQualificationEditModel
            {
                SoldierId = soldierId,
                PossibleQualifications = await GetQuals()
            };

            return View("Edit", model);
        }

        public async Task<ActionResult> Update(int soldierId)
        {
            var possible_quals = await GetQuals();

            var models = from qual in _db.Qualifications
                         join soldier_quals in _db.SoldierQualifications
                            .Where(q => q.SoldierId == soldierId)
                         on qual.Id equals soldier_quals.QualificationId into quals
                         from soldier_qual in quals.DefaultIfEmpty()
                         select new SoldierQualificationEditModel
                         {
                             Id = qual.Id,
                             SoldierId = soldierId,

                             PossibleQualifications = possible_quals,

                             QualificationDate = (soldier_qual != null ? soldier_qual.QualificationDate : DateTime.Today),
                             ExpirationDate = (soldier_qual != null ? soldier_qual.ExpirationDate : null)
                         };

            return View(await models.ToListAsync());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Update(IEnumerable<SoldierQualificationEditModel> models)
        {
            foreach (var model in models)
            {
                await AddOrUpdate(model);
            }

            return RedirectToAction("View", "Soldier", new { soldierId = 1 });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(SoldierQualificationEditModel model)
        {
            if (!ModelState.IsValid)
            {
                model.PossibleQualifications = await GetQuals();
                return View("Edit", model);
            }

            await AddOrUpdate(model);

            return RedirectToAction("View", "Soldier", new { soldierId = model.SoldierId });
        }

        private async Task AddOrUpdate(SoldierQualificationEditModel model)
        {
            if (model.QualificationId == 0) return;
            if (model.SoldierId == 0) return;

            var qual =
                await _db
                .SoldierQualifications
                .Include(s => s.Soldier)
                .Include(q => q.Qualification)
                .Where(s => s.SoldierId == model.SoldierId)
                .Where(q => q.QualificationId == model.QualificationId)
                .SingleOrDefaultAsync();

            if (qual == null)
            {
                qual = _db.SoldierQualifications.Add(new SoldierQualification { SoldierId = model.SoldierId, QualificationId = model.QualificationId });
            }

            qual.Status = model.Status;
            qual.QualificationDate = model.QualificationDate;
            qual.ExpirationDate = model.ExpirationDate;

            await _db.SaveChangesAsync();
        }

        private async Task<IEnumerable<SelectListItem>> GetQuals()
        {
            return
                await _db
                .Qualifications
                .OrderBy(q => q.Name)
                .Select(q => new SelectListItem
                {
                    Text = q.Name,
                    Value = "" + q.Id
                })
                .ToListAsync();
        }
    }
}