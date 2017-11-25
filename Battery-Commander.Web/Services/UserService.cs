using BatteryCommander.Web.Models;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BatteryCommander.Web.Services
{
    public class UserService
    {
        /// <summary>
        /// Find the Soldier associated with the given user.
        /// Will throw an exception if more than Soldier is found.
        /// Will return null if none was found.
        /// </summary>
        public static async Task<Soldier> FindAsync(Database db, ClaimsPrincipal user)
        {
            var email = Get_Email(user);

            var soldiers = await SoldierSearchService.Filter(db, new SoldierSearchService.Query { Email = email });

            if (soldiers.Count() > 1) throw new Exception($"Found multiple matching soldiers with the same email: {email}");

            return soldiers.SingleOrDefault();
        }

        private static String Get_Email(ClaimsPrincipal user) => user?.Identity?.Name;
    }
}