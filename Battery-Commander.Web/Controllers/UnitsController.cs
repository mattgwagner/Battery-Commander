using BatteryCommander.Web.Models;
using BatteryCommander.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BatteryCommander.Web.Controllers
{
    [Authorize, ApiExplorerSettings(IgnoreApi = true)]
    public class UnitsController : Controller
    {
        private readonly Database db;

        public UnitsController(Database db)
        {
            this.db = db;
        }

        [Route("~/Units", Name = "Units.List")]
        public async Task<IActionResult> Index()
        {
            return View("List", await UnitService.List(db, includeIgnored: true));
        }

        [Route("~/Units/{id}", Name = "Unit.Details")]
        public async Task<IActionResult> Details(int id)
        {
            var units = await UnitService.List(db, includeIgnored: true);

            var model = units.Where(_ => _.Id == id).First();

            return View(model);
        }

        public IActionResult New()
        {
            return View("Edit", new Unit { });
        }

        public async Task<IActionResult> Edit(int id)
        {
            return View(await db.Units.FindAsync(id));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(Unit model)
        {
            if (await db.Units.AnyAsync(unit => unit.Id == model.Id) == false)
            {
                db.Units.Add(model);
            }
            else
            {
                db.Units.Update(model);
            }

            await db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public static async Task<IEnumerable<SelectListItem>> GetDropDownList(Database db)
        {
            return
                await db
                .Units
                .OrderBy(unit => unit.Name)
                .Select(unit => new SelectListItem
                {
                    Text = $"{unit.Name}",
                    Value = $"{unit.Id}"
                })
                .ToListAsync();
        }
    }
}