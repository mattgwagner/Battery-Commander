using BatteryCommander.Web.Models;
using BatteryCommander.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using static BatteryCommander.Web.Models.ABCP;

namespace BatteryCommander.Web.Controllers
{
    [Authorize, ApiExplorerSettings(IgnoreApi = true)]
    public class ABCPController : Controller
    {
        private readonly Database db;

        public ABCPController(Database db)
        {
            this.db = db;
        }

        public async Task<IActionResult> Index(SoldierService.Query query)
        {
            return View("List", new ABCPListViewModel
            {
                Soldiers = await SoldierService.Filter(db, query)
            });
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

            return File(abcp.GenerateWorksheet(), "application/pdf", filename);
        }

        public async Task<IActionResult> Counseling(int id)
        {
            var abcp = await Get(db, id);

            var filename = $"{abcp.Soldier.Unit.Name}_DA4856_ABCP_{abcp.Soldier.LastName}_{abcp.Date:yyyyMMdd}.pdf";

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
            if(!ModelState.IsValid)
            {
                ViewBag.Soldiers = await SoldiersController.GetDropDownList(db);

                return View("Edit", model);
            }

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
                .Include(_ => _.Soldier).ThenInclude(_ => _.Unit)
                .Include(_ => _.Soldier).ThenInclude(_ => _.ABCPs)
                .Where(_ => _.Id == id)
                .SingleOrDefaultAsync();
        }
    }
}