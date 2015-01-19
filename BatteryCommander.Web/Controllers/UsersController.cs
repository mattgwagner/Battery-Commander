using BatteryCommander.Common;
using BatteryCommander.Common.Models;
using Microsoft.AspNet.Identity;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BatteryCommander.Web.Controllers
{
    public class UsersController : BaseController
    {
        private readonly DataContext _db;

        public UsersController(UserManager<AppUser, int> userManager, DataContext db)
            : base(userManager)
        {
            _db = db;
        }

        [Route("Users")]
        public async Task<ActionResult> List()
        {
            var users =
                await _db
                .Users
                .ToListAsync();

            return View(users);
        }

        [Route("User/{userId}")]
        public async Task<ActionResult> View(int userId)
        {
            var user =
                await _db
                .Users
                .SingleOrDefaultAsync(w => w.Id == userId);

            return View(user);
        }
    }
}