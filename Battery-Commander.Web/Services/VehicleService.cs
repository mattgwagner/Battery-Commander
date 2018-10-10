using BatteryCommander.Web.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BatteryCommander.Web.Services
{
    public class VehicleService
    {
        public static async Task Reset_Convoy(Database db)
        {
            foreach (var vehicle in await Filter(db, new Query { }))
            {
                vehicle.DriverId = null;
                vehicle.A_DriverId = null;
                vehicle.Chalk = Vehicle.VehicleChalk.Unknown;
                vehicle.OrderOfMarch = 0;

                foreach (var passenger in vehicle.Passengers.ToList())
                {
                    vehicle.Passengers.Remove(passenger);
                }
            }

            await db.SaveChangesAsync();
        }

        public static async Task<IEnumerable<Vehicle>> Filter(Database db, Query query)
        {
            IQueryable<Vehicle> vehicles =
                db
                .Vehicles
                .Include(vehicle => vehicle.Unit)
                .Include(vehicle => vehicle.Driver)
                .Include(vehicle => vehicle.A_Driver)
                .Include(vehicle => vehicle.Passengers)
                .ThenInclude(passenger => passenger.Soldier);

            if (query.Units?.Any() == true)
            {
                vehicles = vehicles.Where(vehicle => query.Units.Contains(vehicle.UnitId));
            }
            else if (query.IncludeIgnoredUnits == false)
            {
                vehicles = vehicles.Where(vehicle => !vehicle.Unit.IgnoreForReports);
            }

            if (query.Available.HasValue)
            {
                vehicles = vehicles.Where(vehicle => vehicle.Available == query.Available);
            }

            return
                await vehicles
                .OrderBy(vehicle => vehicle.Chalk)
                .ThenBy(vehicle => vehicle.OrderOfMarch)
                .ToListAsync();
        }

        public class Query
        {
            public int[] Units { get; set; }

            public Boolean? IncludeIgnoredUnits { get; set; } = false;

            public Boolean? Available { get; set; } = true;
        }
    }
}