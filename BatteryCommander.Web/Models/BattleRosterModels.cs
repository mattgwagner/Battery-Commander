using BatteryCommander.Common.Models;
using System.Collections.Generic;

namespace BatteryCommander.Web.Models
{
    public class BattleRosterModel
    {
        public IEnumerable<Soldier> Soldiers { get; set; }

        public IEnumerable<Qualification> Qualifications { get; set; }
    }
}