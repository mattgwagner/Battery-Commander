using System.Collections.Generic;
using System.Linq;

namespace BatteryCommander.Web.Models
{
    public class APFTListViewModel
    {
        public IEnumerable<Soldier> Soldiers { get; set; } = Enumerable.Empty<Soldier>();

        public int Go => Soldiers.Where(_ => _.APFTs.Any()).Where(_ => _.LastApft.IsPassing).Count();

        public int NoGo => Soldiers.Where(_ => _.APFTs.Any()).Where(_ => !_.LastApft.IsPassing).Count();

        public int Due => Soldiers.Where(_ => !_.APFTs.Any() || !_.LastApft.IsValid).Count();
    }
}