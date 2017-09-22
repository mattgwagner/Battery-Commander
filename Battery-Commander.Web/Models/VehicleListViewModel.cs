using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;

namespace BatteryCommander.Web.Models
{
    public class VehicleListViewModel
    {
        public IEnumerable<SelectListItem> Soldiers { get; set; } = Enumerable.Empty<SelectListItem>();

        public IEnumerable<Vehicle> Vehicles { get; set; } = Enumerable.Empty<Vehicle>();

        public int FMC => Vehicles.Where(_ => _.FMC).Count();

        public int OnHand => Vehicles.Where(_ => _.Status != Vehicle.VehicleStatus.Unknown).Count();

        public int PAX => Vehicles.Where(_ => _.FMC).Where(_ => _.DriverId.HasValue).Count() + Vehicles.Where(_ => _.FMC).Where(_ => _.A_DriverId.HasValue).Count();

        public int Seats => Vehicles.Where(_ => _.FMC).Select(_ => _.Seats).Sum();
    }
}