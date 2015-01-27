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
                .OrderBy(u => u.UserName)
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

        [Route("User/New")]
        [Route("User/{userId}/Edit")]
        public async Task<ActionResult> Edit(int? userId)
        {
            var model = new UserEditModel { };

            var user =
                await _db
                .Users
                .SingleOrDefaultAsync(u => u.Id == userId);

            if (user != null)
            {
                model.Id = user.Id;
                model.UserName = user.UserName;
                model.PhoneNumber = user.PhoneNumber;
                model.TwoFactorEnabled = user.TwoFactorEnabled;
            }

            return View(model);
        }

        public async Task<ActionResult> Save(UserEditModel model)
        {
            if (!ModelState.IsValid) return View("Edit", model);

            var user =
                await _db
                .Users
                .SingleOrDefaultAsync(u => u.Id == model.Id);

            if (user == null)
            {
                user = _db.Users.Add(new AppUser { UserName = model.UserName });
            }

            // TODO Need to handle password creation and other things for new users

            user.PhoneNumber = model.PhoneNumber;
            user.TwoFactorEnabled = model.TwoFactorEnabled;

            await _db.SaveChangesAsync();

            return RedirectToAction("List");
        }
    }
}