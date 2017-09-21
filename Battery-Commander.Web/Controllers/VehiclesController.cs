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
    [Authorize]
    public class VehiclesController : Controller
    {
        private readonly Database db;

        public VehiclesController(Database db)
        {
            this.db = db;
        }

        public async Task<IActionResult> Index()
        {
            // List of Vehicles - by unit, by status

            var vehicles =
                await db
                .Vehicles
                .Include(vehicle => vehicle.Unit)
                .Include(vehicle => vehicle.Driver)
                .Include(vehicle => vehicle.A_Driver)
                .Where(vehicle => !vehicle.Unit.IgnoreForReports)
                .OrderBy(vehicle => vehicle.Bumper)
                .ToListAsync();

            return View("List", vehicles);
        }

        public async Task<IActionResult> New()
        {
            ViewBag.Soldiers = await Get_Available_Drivers(db);
            ViewBag.Units = await UnitsController.GetDropDownList(db);

            return View(nameof(Edit), new Vehicle { });
        }

        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.Soldiers = await Get_Available_Drivers(db);
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

        public static async Task<IEnumerable<SelectListItem>> Get_Available_Drivers(Database db)
        {
            var available_drivers = new List<Soldier>();

            // Current soldiers currently driving

            var current_drivers =
                await db
                .Vehicles
                .Where(vehicle => vehicle.Driver != null)
                .Select(vehicle => vehicle.Driver)
                .Select(driver => driver.Id)
                .ToListAsync();

            // Get all soldiers

            foreach (var soldier in await SoldierSearchService.Filter(db, new SoldierSearchService.Query { }))
            {
                // TODO Remove ones unlicensed

                // Remove ones that are already driving other vehicles

                if (!current_drivers.Contains(soldier.Id))
                {
                    available_drivers.Add(soldier);
                }
            }

            return from soldier in available_drivers
                   orderby soldier.LastName
                   select new SelectListItem
                   {
                       Text = $"{soldier}",
                       Value = $"{soldier.Id}"
                   };
        }
    }
}