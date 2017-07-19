using BatteryCommander.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace BatteryCommander.Web.Controllers
{
    [Authorize]
    public class SSDController : Controller
    {
        private readonly Database db;

        public SSDController(Database db)
        {
            this.db = db;
        }

        public async Task<IActionResult> Index(int? unit)
        {
            var model =
                await db
                .Soldiers
                .Include(s => s.SSDSnapshots)
                .Include(s => s.Unit)
                .Where(s => !s.IsOfficer)
                .Where(s => !s.Unit.IgnoreForReports)
                .Where(s => !unit.HasValue || s.UnitId == unit)
                .OrderBy(s => s.LastName)
                .ThenBy(s => s.FirstName)
                .ToListAsync();

            return View("List", model);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int soldierId, SSD ssd, decimal completion)
        {
            // Take the models and pull the updated data

            var soldier = await SoldiersController.Get(db, soldierId);

            soldier
                .SSDSnapshots
                .Add(new Soldier.SSDSnapshot
                {
                    SSD = ssd,
                    PerecentComplete = completion / 100 // Convert to decimal percentage
                });

            await db.SaveChangesAsync();

            return Json(new { success = true });
        }
    }
}