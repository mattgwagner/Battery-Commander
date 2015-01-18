using BatteryCommander.Common.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Web.Mvc;

namespace BatteryCommander.Web.Controllers
{
    public abstract class BaseController : Controller
    {
        private readonly Lazy<AppUser> _user;

        public AppUser CurrentUser { get { return _user.Value; } }

        protected readonly UserManager<AppUser, int> _userManager;

        public BaseController(UserManager<AppUser, int> userManager)
        {
            _userManager = userManager;

            _user = new Lazy<AppUser>(() => _userManager.FindById(User.Identity.GetUserId<int>()));
        }

        protected void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(String.Empty, error);
            }
        }
    }
}