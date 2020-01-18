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
        public static async Task Reset_Convoy(Database db, Query query)
        {
            foreach (var vehicle in await Filter(db, query))
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
            var vehicles =
                db
                .Vehicles
                .Include(vehicle => vehicle.Unit)
                .Include(vehicle => vehicle.Driver)
                .Include(vehicle => vehicle.A_Driver)
                .Include(vehicle => vehicle.Passengers)
                .ThenInclude(passenger => passenger.Soldier)
                .AsEnumerable();

            if (query.Units?.Any() == true)
            {
                vehicles = vehicles.Where(vehicle => query.Units.Contains(vehicle.UnitId));
            }

            if (query.Available.HasValue)
            {
                vehicles = vehicles.Where(vehicle => vehicle.Available == query.Available);
            }

            return
                vehicles
                .OrderBy(vehicle => vehicle.Chalk)
                .ThenBy(vehicle => vehicle.OrderOfMarch);
        }

        public class Query
        {
            public int[] Units { get; set; }

            public Boolean? Available { get; set; } = true;
        }
    }
}