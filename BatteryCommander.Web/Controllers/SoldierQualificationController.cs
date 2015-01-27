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

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(SoldierQualificationEditModel model)
        {
            if (!ModelState.IsValid)
            {
                model.PossibleQualifications = await GetQuals();
                return View("Edit", model);
            }

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

            return RedirectToAction("View", "Soldier", new { soldierId = model.SoldierId });
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