using BatteryCommander.Common;
using BatteryCommander.Common.Models;
using BatteryCommander.Web.Models;
using Microsoft.AspNet.Identity;
using System;
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

        [Route("~/Users")]
        public async Task<ActionResult> List()
        {
            var users =
                await _db
                .Users
                .OrderBy(u => u.UserName)
                .ToListAsync();

            return View(users);
        }

        [Route("~/User/New")]
        public ActionResult New()
        {
            return View("Edit", new UserEditModel { });
        }

        [Route("~/User/{userId}/Edit")]
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

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(UserEditModel model)
        {
            if (!ModelState.IsValid) return View("Edit", model);

            var user =
                await _db
                .Users
                .SingleOrDefaultAsync(u => u.Id == model.Id);

            if (user == null)
            {
                user = _db.Users.Add(new AppUser
                {
                    UserName = model.UserName
                });
            }

            user.PhoneNumber = model.PhoneNumber;
            user.TwoFactorEnabled = model.TwoFactorEnabled;

            user.SecurityStamp = Guid.NewGuid().ToString();
            user.LastUpdated = DateTime.UtcNow;

            if (!String.IsNullOrWhiteSpace(model.Password))
            {
                model.Password = new PasswordHasher().HashPassword(model.Password);
            }

            await _db.SaveChangesAsync();

            return RedirectToAction("List");
        }
    }
}