using BatteryCommander.Web.Services;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BatteryCommander.Web.Models
{
    public class VehicleListViewModel
    {
        public VehicleService.Query Query { get; set; } = new VehicleService.Query { };

        public IEnumerable<SelectListItem> Soldiers { get; set; } = Enumerable.Empty<SelectListItem>();

        public Array Statuses => Enum.GetNames(typeof(Vehicle.VehicleStatus));

        public Array Locations => Enum.GetNames(typeof(Vehicle.VehicleLocation));

        public IEnumerable<Vehicle> Vehicles { get; set; } = Enumerable.Empty<Vehicle>();

        public int Seats => Vehicles.Where(_ => _.Available).Select(_ => _.Seats).Sum();

        public int FMC => Vehicles.Where(_ => _.Available).Count();

        public int? PAX => Vehicles.Where(_ => _.Available).Select(_ => _.Occupancy).Sum();

        public int Capacity => Vehicles.Where(_ => _.Available).Select(_ => _.TotalCapacity).Sum();
    }
}