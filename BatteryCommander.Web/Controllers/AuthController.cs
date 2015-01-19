using BatteryCommander.Common.Models;
using BatteryCommander.Web.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BatteryCommander.Web.Controllers
{
    public class AuthController : BaseController
    {
        private readonly SignInManager<AppUser, int> SignInManager;

        public AuthController(SignInManager<AppUser, int> signInMgr, UserManager<AppUser, int> userManager)
            : base(userManager)
        {
            SignInManager = signInMgr;
        }

        [HttpGet, AllowAnonymous]
        public ActionResult Login(String returnUrl)
        {
            return View(new LoginModel { ReturnUrl = returnUrl });
        }

        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid) return View(model);

            switch (await SignInManager.PasswordSignInAsync(model.Username, model.Password, isPersistent: model.RememberMe, shouldLockout: true))
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);

                case SignInStatus.LockedOut:
                    ModelState.AddModelError(String.Empty, "Account locked.");
                    return View();

                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });

                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError(String.Empty, "Invalid password.");
                    return View(model);
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            SignInManager.AuthenticationManager.SignOut();

            return RedirectToAction("Index", "Home");
        }

        // TODO - SendCode

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (!String.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }
    }
}