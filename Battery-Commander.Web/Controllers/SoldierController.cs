using BatteryCommander.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace BatteryCommander.Web.Controllers
{
    [Authorize]
    public class SoldierController : Controller
    {
        private readonly Database db;

        public SoldierController(Database db)
        {
            this.db = db;
        }

        public async Task<IActionResult> Index()
        {
            // TODO Filtering by Rank, MOS, Position, Name, Status

            var soldiers =
                await db
                .Soldiers
                .OrderBy(_ => _.LastName)
                .ThenBy(_ => _.FirstName)
                .ToListAsync();

            return View("List", soldiers);
        }

        public async Task<IActionResult> Details(int id)
        {
            return Json(await db.Soldiers.FindAsync(id));
        }

        public IActionResult New()
        {
            return View("Edit", new Soldier { });
        }

        public async Task<IActionResult> Edit(int id)
        {
            return View(await db.Soldiers.FindAsync(id));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(dynamic model)
        {
            // If EXISTS, Update

            var soldier = db.Soldiers.Add(new Soldier
            {
                // Else, Create New
            });

            await db.SaveChangesAsync();

            return RedirectToAction(nameof(Details), model.id);
        }
    }
}