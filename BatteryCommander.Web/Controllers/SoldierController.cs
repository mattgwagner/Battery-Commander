using BatteryCommander.Common;
using BatteryCommander.Common.Models;
using Microsoft.AspNet.Identity;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BatteryCommander.Web.Controllers
{
    public class SoldierController : BaseController
    {
        private readonly DataContext _db;

        public SoldierController(UserManager<AppUser, int> userManager, DataContext db)
            : base(userManager)
        {
            _db = db;
        }

        [Route("Soldiers")]
        public async Task<ActionResult> List()
        {
            var soldiers =
                await _db
                .Soldiers
                .ToListAsync();

            return View(soldiers);
        }

        [Route("Soldier/{soldierId}")]
        public async Task<ActionResult> View(int soldierId)
        {
            var soldier =
                await _db
                .Soldiers
                .Include(s => s.Qualifications)
                .SingleOrDefaultAsync(s => s.Id == soldierId);

            return View(soldier);
        }
    }
}