using BatteryCommander.Web.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace BatteryCommander.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly Database db;

        public HomeController(Database db)
        {
            this.db = db;
        }

        [AllowAnonymous]
        public IActionResult Login(String returnUrl = "/")
        {
            return new ChallengeResult("Auth0", new AuthenticationProperties { RedirectUri = returnUrl });
        }

        [Route("~/Logout")]
        public async Task Logout()
        {
            await HttpContext.Authentication.SignOutAsync("Auth0", new AuthenticationProperties
            {
                RedirectUri = Url.RouteUrl("Units.List")
            });

            await HttpContext.Authentication.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}