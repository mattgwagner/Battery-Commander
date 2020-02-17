using System;
using System.Collections.Generic;
using System.Linq;
using BatteryCommander.Web.Services;

namespace BatteryCommander.Web.Models
{
    public class SoldierListViewModel
    {
        public SoldierService.Query Query { get; set; } = new SoldierService.Query { };

        public IEnumerable<Soldier> Soldiers { get; set; } = Enumerable.Empty<Soldier>();

        public int Count => Soldiers.Count();

        public int Present => Soldiers.Where(_ => _.Status == Soldier.SoldierStatus.PresentForDuty).Count();

        public IEnumerable<SUTA> Upcoming_SUTA_Requests
        {
            get
            {
                return
                    Soldiers
                    .SelectMany(soldier => soldier.SUTAs)
                    .Where(suta => DateTime.Today <= suta.StartDate)
                    .Where(suta => suta.EndDate <= DateTime.Today.AddDays(14))
                    .ToList();
            }
        }
    }
}