using BatteryCommander.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BatteryCommander.Web.Controllers
{
    [Authorize]
    public class EvaluationsController : Controller
    {
        private readonly Database db;

        public EvaluationsController(Database db)
        {
            this.db = db;
        }

        public async Task<IActionResult> Index(Boolean includeComplete = false)
        {
            var evaluations =
                await db
                .Evaluations
                .Where(_ => includeComplete || _.IsCompleted)
                .OrderBy(_ => _.ThruDate)
                .ToListAsync();

            return View("List", evaluations);
        }

        public async Task<IActionResult> Details(int id)
        {
            return Json(await db.Evaluations.FindAsync(id));
        }

        public IActionResult New()
        {
            throw new NotImplementedException();
        }

        public async Task<IActionResult> Edit(int id)
        {
            return View(await db.Evaluations.FindAsync(id));
        }

        public async Task<IActionResult> Save(dynamic model)
        {
            // If EXISTS, Update

            // Else, Create New

            await db.SaveChangesAsync();

            return RedirectToAction(nameof(Details), model.Id);
        }
    }
}