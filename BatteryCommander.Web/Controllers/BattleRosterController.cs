using BatteryCommander.Common;
using BatteryCommander.Common.Models;
using Microsoft.AspNet.Identity;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Linq;
using BatteryCommander.Web.Models;

namespace BatteryCommander.Web.Controllers
{
    public class BattleRosterController : BaseController
    {
        private readonly DataContext _db;

        public BattleRosterController(UserManager<AppUser, int> userManager, DataContext db)
            : base(userManager)
        {
            _db = db;
        }

        public async Task<ActionResult> Roster()
        {
            var roster_items =
                _db
                .Soldiers
                .Include(s => s.Qualifications)
                // TODO How to order soldiers?
                .OrderBy(s => s.LastName)
                .ThenBy(s => s.FirstName)
                .Select(s => new BattleRosterRow
                {
                    // TODO Which qualifications to show?
                });

            return View(roster_items);
        }
    }
}