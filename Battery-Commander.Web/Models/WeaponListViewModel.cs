using System.Collections.Generic;
using System.Linq;

namespace BatteryCommander.Web.Models
{
    public class WeaponListViewModel
    {
        public IEnumerable<Weapon> Weapons { get; set; } = Enumerable.Empty<Weapon>();
    }
}