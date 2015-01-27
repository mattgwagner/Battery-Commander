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

        [Route("Qualification/{qualificationId}/Update")]
        public async Task<ActionResult> Update(int qualificationId)
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

            return View("Bulk", soldier_quals);
        }

        [Route("Qualification/Bulk")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Bulk(IEnumerable<BulkQualificationUpdateModel> models)
        {
            foreach (var model in models)
            {
                await AddOrUpdate(model);
            }

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

        private async Task<SoldierQualification> AddOrUpdate(BulkQualificationUpdateModel model)
        {
            var soldier_qual =
                await _db
                .SoldierQualifications
                .Where(sq => sq.SoldierId == model.SoldierId)
                .Where(sq => sq.QualificationId == model.QualificationId)
                .SingleOrDefaultAsync();

            if (soldier_qual == null)
            {
                soldier_qual = _db.SoldierQualifications.Add(new SoldierQualification
                {
                    SoldierId = model.SoldierId,
                    QualificationId = model.QualificationId
                });
            }

            soldier_qual.QualificationDate = model.QualificationDate;
            soldier_qual.ExpirationDate = model.ExpirationDate;
            soldier_qual.Status = model.Status;

            await _db.SaveChangesAsync();

            return soldier_qual;
        }
    }
}