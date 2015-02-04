﻿using BatteryCommander.Common;
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
    public class QualificationController : BaseController
    {
        private readonly DataContext _db;

        public QualificationController(UserManager<AppUser, int> userManager, DataContext db)
            : base(userManager)
        {
            _db = db;
        }

        [Route("~/Qualifications")]
        public async Task<ActionResult> List()
        {
            var qualifications =
                await _db
                .Qualifications
                .Include(q => q.Tasks)
                .Include(q => q.SoldierQualifications)
                // only show top level quals
                .Where(q => !q.ParentTaskId.HasValue)
                .OrderBy(q => q.Name)
                .ToListAsync();

            return View(qualifications);
        }

        [Route("~/Qualification/{qualificationId}")]
        public async Task<ActionResult> View(int qualificationId)
        {
            var qualification =
                await _db
                .Qualifications
                .Include(q => q.ParentTask)
                .Include(q => q.Tasks)
                .SingleOrDefaultAsync(q => q.Id == qualificationId);

            return View(qualification);
        }

        [Route("~/Qualification/{qualificationId}/Edit")]
        public async Task<ActionResult> Edit(int? qualificationId)
        {
            var model = new QualificationEditModel { };

            var qualification =
                await _db
                .Qualifications
                .SingleOrDefaultAsync(q => q.Id == qualificationId);

            model.PossibleParentQualifications = from q in _db.Qualifications
                                                 select new SelectListItem
                                                 {
                                                     Text = q.Name,
                                                     Value = q.Id + ""
                                                 };

            if (qualification != null)
            {
                model.Id = qualification.Id;
                model.Name = qualification.Name;
                model.Description = qualification.Description;
                model.ParentTaskId = qualification.ParentTaskId;
            }

            return View(model);
        }

        [Route("~/Qualification/New")]
        public ActionResult New(int? parentId)
        {
            var model = new QualificationEditModel
            {
                ParentTaskId = parentId,
                PossibleParentQualifications = from q in _db.Qualifications
                                               select new SelectListItem
                                               {
                                                   Text = q.Name,
                                                   Value = q.Id + ""
                                               }
            };

            return View("Edit", model);
        }

        [Route("~/Qualification/{qualificationId}/Update")]
        public async Task<ActionResult> Update(int qualificationId, int? rankId, int? positionId, int? groupId)
        {
            var qualification =
                await _db
                .Qualifications
                .SingleAsync(q => q.Id == qualificationId);

            ViewBag.Qualification = qualification;

            var soldier_quals = from soldier in _db.Soldiers
                                join qualifications in _db.SoldierQualifications
                                    .Where(q => q.QualificationId == qualificationId)
                                on soldier.Id equals qualifications.SoldierId into quals
                                from soldier_qual in quals.DefaultIfEmpty()
                                where
                                (
                                    soldier.Status == SoldierStatus.Active
                                    && (!rankId.HasValue || soldier.Rank == (Rank)rankId.Value)
                                    && (!positionId.HasValue || soldier.Position == (Position)positionId.Value)
                                    && (!groupId.HasValue || soldier.Group == (Group)groupId.Value)
                                )
                                orderby soldier.LastName
                                select new BulkQualificationUpdateModel
                                {
                                    QualificationId = qualificationId,
                                    SoldierId = soldier.Id,
                                    Soldier = soldier,

                                    QualificationDate = (soldier_qual != null ? soldier_qual.QualificationDate : DateTime.Today),
                                    ExpirationDate = (soldier_qual != null ? soldier_qual.ExpirationDate : null),
                                    Status = (soldier_qual != null ? soldier_qual.Status : QualificationStatus.Unknown),
                                    Comments = (soldier_qual != null ? soldier_qual.Comments : String.Empty)
                                };

            return View("Update", soldier_quals);
        }

        [Route("~/Qualifications/Save")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Update(IEnumerable<BulkQualificationUpdateModel> models)
        {
            foreach (var model in models)
            {
                await AddOrUpdate(model);
            }

            return RedirectToAction("List");
        }

        [Route("~/Qualification/Save")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(QualificationEditModel model)
        {
            var qualification =
                await _db
                .Qualifications
                .SingleOrDefaultAsync(q => q.Id == model.Id);

            if (qualification == null)
            {
                qualification = _db.Qualifications.Add(new Qualification { });
            }

            qualification.ParentTaskId = model.ParentTaskId;
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
            soldier_qual.Comments = model.Comments;

            await _db.SaveChangesAsync();

            return soldier_qual;
        }

        [Route("~/Qualification/{qualificationId}/Tasks")]
        public ActionResult AddTasks(int qualificationId)
        {
            return View(Enumerable.Range(1, count: 5).Select(i => new QualificationEditModel { ParentTaskId = qualificationId }));
        }

        [Route("~/Qualification/Save/Tasks")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> AddTasksSave(IEnumerable<QualificationEditModel> tasksToAdd)
        {
            foreach (var task in tasksToAdd)
            {
                if (!String.IsNullOrWhiteSpace(task.Name))
                {
                    await Save(task);
                }
            }

            return RedirectToAction("List");
        }

        [Route("~/Qualification/{qualificationId}/Delete")]
        public async Task<ActionResult> Delete(int qualificationId)
        {
            var qual = await _db.Qualifications.SingleAsync(q => q.Id == qualificationId);

            int? parentId = qual.ParentTaskId;

            _db.Entry(qual).State = EntityState.Deleted;

            await _db.SaveChangesAsync();

            if (parentId.HasValue)
            {
                return RedirectToAction("View", new { qualificationId = parentId });
            }

            return RedirectToAction("List");
        }
    }
}