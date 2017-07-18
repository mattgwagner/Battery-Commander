using BatteryCommander.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using static BatteryCommander.Web.Models.ABCP;

namespace BatteryCommander.Web.Controllers
{
    [Authorize]
    public class ABCPController : Controller
    {
        private readonly Database db;

        public ABCPController(Database db)
        {
            this.db = db;
        }

        public async Task<IActionResult> Index(int? unit = null, DateTime? date = null)
        {
            // TODO Filtering by pass/fail

            var tests =
                await db
                .ABCPs
                .Where(abcp => !date.HasValue || abcp.Date.Date == date.Value)
                .Where(abcp => !unit.HasValue || abcp.Soldier.UnitId == unit)
                .OrderByDescending(abcp => abcp.Date)
                .Include(abcp => abcp.Soldier)
                .Include(abcp => abcp.Soldier.Unit)
                .ToListAsync();

            return View("List", tests);
        }

        public async Task<IActionResult> Details(int id)
        {
            return View(await Get(db, id));
        }

        public async Task<IActionResult> Worksheet(int id)
        {
            // TODO It's a 5501 for females, and that's just the worksheet, not the associated counseling

            var abcp = await Get(db, id);

            var filename = $"{abcp.Soldier.Unit.Name}_DA5500_ABCP_{abcp.Soldier.LastName}_{abcp.Date:yyyyMMdd}.pdf";

            return File(abcp.GenerateCounseling(), "application/pdf", filename);
        }

        public async Task<IActionResult> Measurements(int id)
        {
            return View(await Get(db, id));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Measurements(int id, Measurement[] measurements)
        {
            var abcp = await Get(db, id);

            abcp.Measurements = measurements;

            await db.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id });
        }

        public async Task<IActionResult> New(int soldier = 0)
        {
            ViewBag.Soldiers = await SoldiersController.GetDropDownList(db);

            return View(nameof(Edit), new ABCP { SoldierId = soldier });
        }

        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.Soldiers = await SoldiersController.GetDropDownList(db);

            return View(await Get(db, id));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(ABCP model)
        {
            if (await db.ABCPs.AnyAsync(abcp => abcp.Id == model.Id) == false)
            {
                db.ABCPs.Add(model);
            }
            else
            {
                db.ABCPs.Update(model);
            }

            await db.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { model.Id });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var abcp = await Get(db, id);

            db.ABCPs.Remove(abcp);

            await db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public static async Task<ABCP> Get(Database db, int id)
        {
            return
                await db
                .ABCPs
                .Include(_ => _.Soldier)
                .ThenInclude(_ => _.Unit)
                .Where(_ => _.Id == id)
                .SingleOrDefaultAsync();
        }
    }
}