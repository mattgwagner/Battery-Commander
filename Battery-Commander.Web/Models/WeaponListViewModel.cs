using System.Collections.Generic;
using System.Linq;

namespace BatteryCommander.Web.Models
{
    public class WeaponListViewModel
    {
        public IEnumerable<Weapon> Weapons { get; set; } = Enumerable.Empty<Weapon>();

        public IEnumerable<KeyValuePair<Weapon.WeaponType, int>> Count_By_Type
        {
            get
            {
                return
                    Weapons
                    .GroupBy(weapon => weapon.Type)
                    .Select(grouped => KeyValuePair.Create(grouped.Key, grouped.Count()));
            }
        }
    }
}