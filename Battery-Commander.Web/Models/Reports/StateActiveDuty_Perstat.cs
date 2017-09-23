using System.Collections.Generic;
using System.Linq;

namespace BatteryCommander.Web.Models.Reports
{
    public class StateActiveDuty_Perstat
    {
        // This is probably a State of Florida-specific State Active Duty format that doesn't make a lot of sense

        public IEnumerable<Soldier> Soldiers { get; set; } = Enumerable.Empty<Soldier>();

        public int Assigned => Soldiers.Count();

        public int WeaponsQualified => Soldiers.Where(_ => _.IwqQualified).Count();

        public int DscaQualified => Soldiers.Where(_ => _.DscaQualified).Count();

        public int BothWeaponAndDscaQualified => Soldiers.Where(_ => _.IwqQualified).Where(_ => _.DscaQualified).Count();

        // public int Commited { get; set; }

        // public int Uncommited { get; set; }

        // public int Mobilized { get; set; }

        // public int Available { get; set; }
    }
}