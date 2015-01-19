using BatteryCommander.Common;
using BatteryCommander.Common.Models;
using Microsoft.AspNet.Identity;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BatteryCommander.Web.Controllers
{
    public class GroupController : BaseController
    {
        private readonly DataContext _db;

        public GroupController(UserManager<AppUser, int> userManager, DataContext db)
            : base(userManager)
        {
            _db = db;
        }

        [Route("Groups")]
        public async Task<ActionResult> List()
        {
            var groups =
                await _db
                .Groups
                .Include(g => g.Leader)
                .ToListAsync();

            return View(groups);
        }

        [Route("Group/{groupId}")]
        public async Task<ActionResult> View(int groupId)
        {
            var group =
                await _db
                .Groups
                .Include(g => g.Leader)
                .Include(g => g.Soldiers)
                .SingleOrDefaultAsync(g => g.Id == groupId);

            return View(group);
        }
    }
}