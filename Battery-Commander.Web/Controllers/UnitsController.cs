using BatteryCommander.Web.Models;
using BatteryCommander.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
            var model = await UnitService.Get(db, id);

            ViewBag.CalendarUrl = CalendarService.GenerateUrl(User, Url, id);

            return View(model);
        }

        [Route("~/Units/New")]
        public IActionResult New()
        {
            return View("Edit", new Unit { });
        }

        [Route("~/Units/{id}/Edit")]
        public async Task<IActionResult> Edit(int id)
        {
            return View(await db.Units.FindAsync(id));
        }

        [Route("Save"), HttpPost, ValidateAntiForgeryToken]
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

        [AllowAnonymous, Route("~/Units/{unitId}/Calendar"), ResponseCache(Duration = 0)]
        public async Task<IActionResult> Calendar([FromRoute]int unitId, [FromQuery]String apiKey)
        {
            ClaimsPrincipal user;

            if (!UserService.Try_Validate_Token(apiKey, out user))
            {
                throw new Exception("Unable to validate apiKey");
            }

            Log.Information("Generating Calendar Feed for {User}", UserService.Get_Email(user));

            var data = await CalendarService.Generate(this.db, unitId);

            return File(data, "text/calendar");
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