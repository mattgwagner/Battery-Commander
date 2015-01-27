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

namespace BatteryCommander.Web.Controllers
{
    [RoutePrefix("Soldier")]
    public class SoldierQualificationController : BaseController
    {
        private readonly DataContext _db;

        public SoldierQualificationController(UserManager<AppUser, int> userManager, DataContext db)
            : base(userManager)
        {
            _db = db;
        }

        [Route("{soldierId}/Qualification/{qualificationId}")]
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

        [Route("{soldierId}/Qualification/New")]
        public async Task<ActionResult> New(int soldierId)
        {
            var model = new SoldierQualificationEditModel
            {
                SoldierId = soldierId,
                PossibleQualifications = await GetQuals()
            };

            return View("Edit", model);
        }


        [Route("{soldierId}/Qualification/Update")]
        public async Task<ActionResult> Update(int soldierId)
        {
            var lines_to_add = 10;

            var possible_qualifications = await GetQuals();

            var quals =
                _db
                .SoldierQualifications
                .Where(q => q.SoldierId == soldierId)
                .AsEnumerable()
                .Select(m => new SoldierQualificationEditModel
                {
                    QualificationId = m.QualificationId,
                    SoldierId = m.SoldierId,
                    Status = m.Status,
                    QualificationDate = m.QualificationDate,
                    ExpirationDate = m.ExpirationDate,
                    PossibleQualifications = possible_qualifications
                })
                .ToList();

            quals.AddRange(Enumerable.Range(1, lines_to_add).Select(s => new SoldierQualificationEditModel { SoldierId = soldierId, PossibleQualifications = possible_qualifications }));

            return View(quals);
        }

        [Route("Qualifications/Update")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Update_All(IEnumerable<SoldierQualificationEditModel> models)
        {
            foreach (var model in models)
            {
                await AddOrUpdate(model);
            }

            return RedirectToAction("View", "Soldier", new { soldierId = 1 });
        }

        [Route("Qualification/Save")]
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
            if (model.Status == QualificationStatus.Unknown && model.QualificationDate == DateTime.Today) return;

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