using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;

namespace BatteryCommander.Web.Models
{
    public class VehicleListViewModel
    {
        public IEnumerable<SelectListItem> Soldiers { get; set; } = Enumerable.Empty<SelectListItem>();

        public IEnumerable<Vehicle> Vehicles { get; set; } = Enumerable.Empty<Vehicle>();

        private IEnumerable<Vehicle> fmc_vehicles => Vehicles.Where(_ => _.Status == Vehicle.VehicleStatus.FMC);

        private int drivers => fmc_vehicles.Where(_ => _.DriverId.HasValue).Count();

        private int adrivers => fmc_vehicles.Where(_ => _.A_DriverId.HasValue).Count();

        public int FMC => fmc_vehicles.Count();

        public int PAX => drivers + adrivers;

        public int Seats => fmc_vehicles.Select(_ => _.Seats).Sum();
    }
}