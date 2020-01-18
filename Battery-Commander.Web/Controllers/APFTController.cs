using BatteryCommander.Web.Models;
using BatteryCommander.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace BatteryCommander.Web.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class APFTController : Controller
    {
        private readonly Database db;

        public APFTController(Database db)
        {
            this.db = db;
        }

        public async Task<IActionResult> Index(SoldierService.Query query)
        {
            return View("List", new APFTListViewModel
            {
                Soldiers = await SoldierService.Filter(db, query)
            });
        }

        public async Task<IActionResult> Details(int id)
        {
            return View(await Get(db, id));
        }

        public async Task<IActionResult> Counseling(int id)
        {
            var apft = await Get(db, id);

            var filename = $"{apft.Soldier.Unit.Name}_DA4856_APFT_{apft.Soldier.LastName}_{apft.Date:yyyyMMdd}.pdf";

            return File(apft.GenerateCounseling(), "application/pdf", filename);
        }

        public async Task<IActionResult> New(int soldier = 0)
        {
            ViewBag.Soldiers = await SoldiersController.GetDropDownList(db, SoldierService.Query.ALL);

            return View(nameof(Edit), new APFT { SoldierId = soldier });
        }

        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.Soldiers = await SoldiersController.GetDropDownList(db, SoldierService.Query.ALL);

            return View(await Get(db, id));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(APFT model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Soldiers = await SoldiersController.GetDropDownList(db, SoldierService.Query.ALL);

                return View("Edit", model);
            }

            if (await db.APFTs.AnyAsync(apft => apft.Id == model.Id) == false)
            {
                db.APFTs.Add(model);
            }
            else
            {
                db.APFTs.Update(model);
            }

            await db.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { model.Id });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var apft = await Get(db, id);

            if (apft == null) return NotFound();

            db.APFTs.Remove(apft);

            await db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public static async Task<APFT> Get(Database db, int id)
        {
            return
                await db
                .APFTs
                .Include(_ => _.Soldier)
                .ThenInclude(_ => _.Unit)
                .Where(_ => _.Id == id)
                .SingleOrDefaultAsync();
        }
    }
}