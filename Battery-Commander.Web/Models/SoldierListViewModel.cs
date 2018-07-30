using BatteryCommander.Web.Services;
using System.Collections.Generic;
using System.Linq;

namespace BatteryCommander.Web.Models
{
    public class SoldierListViewModel
    {
        public SoldierService.Query Query { get; set; } = new SoldierService.Query { };

        public IEnumerable<Soldier> Soldiers { get; set; } = Enumerable.Empty<Soldier>();

        public int Count => Soldiers.Count();

        public int Present => Soldiers.Where(_ => _.Status == Soldier.SoldierStatus.PresentForDuty).Count();
    }
}