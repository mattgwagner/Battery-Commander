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

        // List of Vehicles - by unit, by status

        // Add Vehicle

        // Edit Vehicle

        // Delete Vehicle (?)

        public async Task<IActionResult> Index(int? unit = null)
        {
            var vehicles =
                await db
                .Vehicles
                .Include(vehicle => vehicle.Unit)
                .ToListAsync();

            return Json(vehicles);
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