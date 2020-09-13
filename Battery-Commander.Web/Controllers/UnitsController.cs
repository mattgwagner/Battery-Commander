using BatteryCommander.Web.Models;
using BatteryCommander.Web.Queries;
using BatteryCommander.Web.Services;
using MediatR;
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

using Unit = BatteryCommander.Web.Models.Unit;

namespace BatteryCommander.Web.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class UnitsController : Controller
    {
        private readonly Database db;
        private readonly IMediator dispatcher;

        public UnitsController(Database db, IMediator dispatcher)
        {
            this.db = db;
            this.dispatcher = dispatcher;
        }

        [Route("~/Units", Name = "Units.List")]
        public async Task<IActionResult> Index()
        {
            return View("List", await UnitService.List(db));
        }

        [Route("~/")]
        public async Task<IActionResult> Home()
        {
            var user = await dispatcher.Send(new GetCurrentUser { });

            if (user?.UnitId > 0)
            {
                return RedirectToAction(nameof(Details), new { id = user.UnitId });
            }

            return RedirectToRoute("Units.List");
        }

        [Route("~/Units/{id}", Name = "Unit.Details")]
        public async Task<IActionResult> Details(int id)
        {
            var model = await UnitService.Get(db, id);

            ViewBag.CalendarUrl = CalendarService.GenerateUrl(User, Url, id);

            return View("Details", model);
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

        [AllowAnonymous, Route("~/Calendar/{unitId}"), ResponseCache(Duration = 0)]
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
