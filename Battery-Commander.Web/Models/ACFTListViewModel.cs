using System.Collections.Generic;
using System.Linq;

namespace BatteryCommander.Web.Models
{
    public class ACFTListViewModel
    {
        public IEnumerable<Soldier> Soldiers { get; set; } = Enumerable.Empty<Soldier>();

        public int Go => Soldiers.Where(_ => _.ACFTs.Any()).Where(_ => _.LastAcft.IsPassing).Count();

        public int NoGo => Soldiers.Where(_ => _.ACFTs.Any()).Where(_ => !_.LastAcft.IsPassing).Count();

        public int Due => Soldiers.Where(_ => !_.ACFTs.Any() || !_.LastAcft.IsValid).Count();
    }
}