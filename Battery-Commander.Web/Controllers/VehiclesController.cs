using BatteryCommander.Web.Models;
using BatteryCommander.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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

        [AllowAnonymous]
        public async Task<IActionResult> Index(VehicleService.Query query)
        {
            // List of Vehicles - by unit, by status

            return View("List", new VehicleListViewModel
            {
                Query = query,
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
                .Include(vehicle => vehicle.Passengers)
                .ThenInclude(passenger => passenger.Soldier)
                .Where(_ => _.Id == id)
                .SingleOrDefaultAsync();

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(Vehicle model, IEnumerable<int> passengers)
        {
            if (await db.Vehicles.AnyAsync(vehicles => vehicles.Id == model.Id) == false)
            {
                if (await db.Vehicles.AnyAsync(vehicle => vehicle.UnitId == model.UnitId && vehicle.Bumper == model.Bumper))
                {
                    ModelState.AddModelError(nameof(model.Bumper), "Vehicle exists for this unit and bumper");
                    return View("Edit", model);
                }

                db.Vehicles.Add(model);
            }
            else
            {
                db.Vehicles.Update(model);
            }

            Reassign_Passengers(model.Id, model.DriverId, model.A_DriverId, passengers);

            await db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> SetDriver(int vehicleId, int? driverId, int? adriverId)
        {
            Reassign_Passengers(vehicleId, driverId, adriverId, Enumerable.Empty<int>());

            await db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
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
        public async Task<IActionResult> Reset(VehicleService.Query query)
        {
            await VehicleService.Reset_Convoy(db, query);

            return RedirectToAction(nameof(Index), query);
        }

        private void Reassign_Passengers(int vehicleId, int? driverId, int? adriverId, IEnumerable<int> passengers)
        {
            foreach (var vehicle in db.Vehicles.Include(_ => _.Passengers))
            {
                if (vehicle.Id == vehicleId)
                {
                    vehicle.DriverId = driverId;
                    vehicle.A_DriverId = adriverId;

                    foreach (var passenger in passengers)
                    {
                        // Add in any passenger we don't already have

                        if (!vehicle.Passengers.Any(existing => existing.SoldierId == passenger))
                        {
                            vehicle.Passengers.Add(new Vehicle.Passenger { SoldierId = passenger });
                        }
                    }

                    // Remove any soldier that was listed as a passenger that's not in the current list

                    foreach (var passenger in vehicle.Passengers.Where(passenger => !passengers.Contains(passenger.SoldierId)).ToList())
                    {
                        vehicle.Passengers.Remove(passenger);
                    }
                }
                else
                {
                    // TODO This is sloppy, should probably be a better way

                    if (vehicle.DriverId == driverId) vehicle.DriverId = null;
                    if (vehicle.A_DriverId == driverId) vehicle.A_DriverId = null;

                    if (vehicle.DriverId == adriverId) vehicle.DriverId = null;
                    if (vehicle.A_DriverId == adriverId) vehicle.A_DriverId = null;

                    if (passengers.Any())
                    {
                        // We added this soldier to another vehicle, remove them from this one

                        foreach (var passenger in vehicle.Passengers.Where(passenger => passengers.Contains(passenger.SoldierId)).ToList())
                        {
                            vehicle.Passengers.Remove(passenger);
                        }
                    }
                }
            }
        }
    }
}