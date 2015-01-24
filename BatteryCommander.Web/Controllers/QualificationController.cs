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
    public class QualificationController : BaseController
    {
        private readonly DataContext _db;

        public QualificationController(UserManager<AppUser, int> userManager, DataContext db)
            : base(userManager)
        {
            _db = db;
        }

        [Route("Qualifications")]
        public async Task<ActionResult> List()
        {
            var qualifications =
                await _db
                .Qualifications
                .ToListAsync();

            return View(qualifications);
        }

        [Route("Qualification/{qualificationId}")]
        public async Task<ActionResult> View(int qualificationId)
        {
            var qualification =
                await _db
                .Qualifications
                .SingleOrDefaultAsync(q => q.Id == qualificationId);

            return View(qualification);
        }

        [Route("Qualification/New")]
        [Route("Qualification/{qualificationId}/Edit")]
        public async Task<ActionResult> Edit(int? qualificationId)
        {
            var model = new QualificationEditModel { };

            var qualification =
                await _db
                .Qualifications
                .SingleOrDefaultAsync(q => q.Id == qualificationId);

            if (qualification != null)
            {
                model.Id = qualification.Id;
                model.Name = qualification.Name;
                model.Description = qualification.Description;
            }

            return View(model);
        }

        [Route("Qualification/{qualificationId}/Bulk")]
        public async Task<ActionResult> Bulk(int qualificationId)
        {
            var qualification =
                await _db
                .Qualifications
                .SingleOrDefaultAsync(q => q.Id == qualificationId);

            var soldier_quals = from soldier in _db.Soldiers
                                join qualifications in _db.SoldierQualifications
                                    .Where(q => q.QualificationId == qualificationId)
                                on soldier.Id equals qualifications.SoldierId into quals
                                from soldier_qual in quals.DefaultIfEmpty()
                                select new BulkQualificationUpdateModel
                                {
                                    QualificationId = qualificationId,
                                    SoldierId = soldier.Id,
                                    Soldier = soldier,

                                    QualificationDate = (soldier_qual != null ? soldier_qual.QualificationDate : DateTime.Today),
                                    ExpirationDate = (soldier_qual != null ? soldier_qual.ExpirationDate : null),
                                    Status = (soldier_qual != null ? soldier_qual.Status : QualificationStatus.Unknown)
                                };

            return View(soldier_quals);
        }

        [Route("Qualification/Bulk")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Bulk(IEnumerable<BulkQualificationUpdateModel> models)
        {
            // TODO

            return RedirectToAction("List");
        }

        [Route("Qualification/Save")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(QualificationEditModel model)
        {
            if (!ModelState.IsValid) return View("Edit", model);

            var qualification =
                await _db
                .Qualifications
                .SingleOrDefaultAsync(q => q.Id == model.Id);

            if (qualification == null)
            {
                _db.Qualifications.Add(new Qualification { });
            }

            qualification.Name = model.Name;
            qualification.Description = model.Description;

            await _db.SaveChangesAsync();

            return RedirectToAction("View", new { qualificationId = qualification.Id });
        }
    }
}