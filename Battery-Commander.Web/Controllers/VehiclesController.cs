using BatteryCommander.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace BatteryCommander.Web.Controllers
{
    [Authorize]
    public class VehiclesController : Controller
    {
        private readonly Database db;

        public VehiclesController(Database db)
        {
            this.db = db;
        }

        // Add Vehicle

        // Edit Vehicle

        // Delete Vehicle (?)

        public async Task<IActionResult> Index()
        {
            // List of Vehicles - by unit, by status

            var vehicles =
                await db
                .Vehicles
                .Include(vehicle => vehicle.Unit)
                .Where(vehicle => !vehicle.Unit.IgnoreForReports)
                .OrderBy(vehicle => vehicle.Bumper)
                .ToListAsync();

            return View("List", vehicles);
        }

        public async Task<IActionResult> New()
        {
            ViewBag.Soldiers = await SoldiersController.GetDropDownList(db);
            ViewBag.Units = await UnitsController.GetDropDownList(db);

            return View(nameof(Edit), new Vehicle { });
        }

        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.Soldiers = await SoldiersController.GetDropDownList(db);
            ViewBag.Units = await UnitsController.GetDropDownList(db);

            var model =
                await db
                .Vehicles
                .Where(_ => _.Id == id)
                .SingleOrDefaultAsync();

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(Vehicle model)
        {
            if (await db.Vehicles.AnyAsync(vehicles => vehicles.Id == model.Id) == false)
            {
                db.Vehicles.Add(model);
            }
            else
            {
                db.Vehicles.Update(model);
            }

            await db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var vehicle = await db
                .Vehicles
                .Where(_ => _.Id == id)
                .SingleOrDefaultAsync();

            db.Vehicles.Remove(vehicle);

            await db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}