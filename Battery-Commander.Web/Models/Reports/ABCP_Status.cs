using System.Collections.Generic;
using System.Linq;

namespace BatteryCommander.Web.Models.Reports
{
    public class ABCP_Status : Stat
    {
        public IEnumerable<Soldier> Soldiers { get; set; } = Enumerable.Empty<Soldier>();

    }
}