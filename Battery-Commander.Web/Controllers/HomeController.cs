using BatteryCommander.Web.Models;
using BatteryCommander.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BatteryCommander.Web.Controllers
{
    [Authorize, ApiExplorerSettings(IgnoreApi = true)]
    public class HomeController : Controller
    {
        private readonly Database db;

        private async Task<Soldier> CurrentUser() => await UserService.FindAsync(db, User);

        public HomeController(Database db)
        {
            this.db = db;
        }

        public async Task<ActionResult> Index()
        {
            var user = await CurrentUser();

            if (user?.UnitId > 0)
            {
                return RedirectToRoute("Unit.Details", new { id = user.UnitId });
            }

            return RedirectToRoute("Units.List");
        }

        public ActionResult PrivacyAct()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            return new ChallengeResult("Auth0", new AuthenticationProperties
            {
                RedirectUri = Url.Action(nameof(PrivacyAct))
            });
        }

        [Route("~/Logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();

            return RedirectToAction(nameof(Login));
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}