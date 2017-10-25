using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;

namespace BatteryCommander.Web.Models
{
    public class VehicleListViewModel
    {
        public IEnumerable<SelectListItem> Soldiers { get; set; } = Enumerable.Empty<SelectListItem>();

        public IEnumerable<Vehicle> Vehicles { get; set; } = Enumerable.Empty<Vehicle>();

        protected IEnumerable<Vehicle> Available => Vehicles.Where(_ => _.FMC).Where(_ => _.Location == Vehicle.VehicleLocation.HS);

        public int FMC => Available.Count();

        public int PAX => Available.Where(_ => _.DriverId.HasValue).Count() + Available.Where(_ => _.FMC).Where(_ => _.A_DriverId.HasValue).Count();

        public int Seats => Available.Select(_ => _.Seats).Sum();
    }
}