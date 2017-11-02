using System.Collections.Generic;
using System.Linq;

namespace BatteryCommander.Web.Models
{
    public class ABCPListViewModel
    {
        public IEnumerable<Soldier> Soldiers { get; set; } = Enumerable.Empty<Soldier>();

        public int Go => Soldiers.Where(_ => _.ABCPs.Any()).Where(_ => _.LastAbcp.IsPassing).Count();

        public int NoGo => Soldiers.Where(_ => _.ABCPs.Any()).Where(_ => !_.LastAbcp.IsPassing).Count();

        public int Due => Soldiers.Where(_ => !_.ABCPs.Any() || !_.LastAbcp.IsValid).Count();
    }
}