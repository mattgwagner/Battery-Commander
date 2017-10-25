using BatteryCommander.Web.Models;
using BatteryCommander.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BatteryCommander.Web.Controllers
{
    [Authorize, ApiExplorerSettings(IgnoreApi = true)]
    public class VehiclesController : Controller
    {
        private readonly Database db;

        public VehiclesController(Database db)
        {
            this.db = db;
        }

        public async Task<IActionResult> Index(VehicleService.Query query)
        {
            // List of Vehicles - by unit, by status

            return View("List", new VehicleListViewModel
            {
                Soldiers = await SoldiersController.GetDropDownList(db, includeIgnoredUnits: false),
                Vehicles = await VehicleService.Filter(db, query)
            });
        }

        public async Task<IActionResult> New()
        {
            ViewBag.Soldiers = await SoldiersController.GetDropDownList(db, includeIgnoredUnits: false);
            ViewBag.Units = await UnitsController.GetDropDownList(db);

            return View(nameof(Edit), new Vehicle { });
        }

        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.Soldiers = await SoldiersController.GetDropDownList(db, includeIgnoredUnits: false);
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

            Reassign_Passengers(model.Id, model.DriverId, model.A_DriverId);

            await db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> SetDriver(int vehicleId, int? driverId, int? adriverId)
        {
            Reassign_Passengers(vehicleId, driverId, adriverId);

            await db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> SetStatus(int vehicleId, Vehicle.VehicleStatus status, Vehicle.VehicleLocation location, String notes)
        {
            var vehicle =
                await db
                .Vehicles
                .Where(_ => _.Id == vehicleId)
                .SingleOrDefaultAsync();

            vehicle.Status = status;
            vehicle.Notes = notes;
            vehicle.Location = location;

            if (Vehicle.VehicleStatus.FMC != status)
            {
                // Changing to !FMC, remove passengers

                vehicle.DriverId = null;
                vehicle.A_DriverId = null;
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

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Reset()
        {
            await VehicleService.Reset_Drivers(db);

            return RedirectToAction(nameof(Index));
        }

        private void Reassign_Passengers(int vehicleId, int? driverId, int? adriverId)
        {
            foreach (var vehicle in db.Vehicles)
            {
                if (vehicle.Id == vehicleId)
                {
                    vehicle.DriverId = driverId;
                    vehicle.A_DriverId = adriverId;
                }
                else
                {
                    if (vehicle.DriverId == driverId) vehicle.DriverId = null;
                    if (vehicle.A_DriverId == adriverId) vehicle.A_DriverId = null;
                }

                // TODO Handle passengers
            }
        }
    }
}