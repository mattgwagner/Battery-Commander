using BatteryCommander.Common;
using BatteryCommander.Common.Models;
using BatteryCommander.Web.Models;
using Microsoft.AspNet.Identity;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

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

        [Route("BattleRoster")]
        public async Task<ActionResult> Show()
        {
            // TODO How to order soldiers?

            var roster_items =
                await _db
                .Soldiers
                .Include(s => s.Qualifications)
                .Where(s => s.Status == SoldierStatus.Active)
                .OrderBy(s => s.Rank)
                .ThenBy(s => s.LastName)
                .Select(soldier => new BattleRosterRow
                {
                    Soldier = soldier,
                    Position = "Superman"

                    // TODO Which qualifications to show?
                })
                .ToListAsync();

            return View(roster_items);
        }
    }
}