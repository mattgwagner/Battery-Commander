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
        public static async Task Reset_Drivers(Database db)
        {
            foreach (var vehicle in db.Vehicles)
            {
                vehicle.DriverId = null;
                vehicle.A_DriverId = null;
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
                .Include(vehicle => vehicle.A_Driver);

            if (query.Unit.HasValue)
            {
                vehicles = vehicles.Where(vehicle => vehicle.UnitId == query.Unit);
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
                .OrderBy(vehicle => vehicle.Bumper)
                .ToListAsync();
        }

        public class Query
        {
            public int? Unit { get; set; }

            public Boolean? IncludeIgnoredUnits { get; set; } = false;

            public Boolean? Available { get; set; }
        }
    }
}