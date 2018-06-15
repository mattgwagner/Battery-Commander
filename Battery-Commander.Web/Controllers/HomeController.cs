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

        public IActionResult Index()
        {
            return RedirectToRoute("Units.List");
        }

        public async Task<ActionResult> PrivacyAct()
        {
            var user = await CurrentUser();

            if(user?.UnitId > 0)
            {
                ViewBag.RedirectUrl = Url.RouteUrl("Unit.Details", new { id = user.UnitId });
            }
            else
            {
                ViewBag.RedirectUrl = Url.Action(nameof(Index));
            }

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