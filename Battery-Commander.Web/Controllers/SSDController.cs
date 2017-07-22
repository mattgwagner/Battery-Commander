using BatteryCommander.Web.Models;
using BatteryCommander.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
            return View("List", await SoldierSearchService.Filter(db, new SoldierSearchService.Query
            {
                Unit = unit,
                OnlyEnlisted = true
            }));
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

            return RedirectToAction(nameof(Index));
        }
    }
}