using BatteryCommander.Web.Models;
using BatteryCommander.Web.Services;
using FluentEmail.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BatteryCommander.Web.Controllers
{
    [Authorize, ApiExplorerSettings(IgnoreApi = true)]
    public class HomeController : Controller
    {
        private readonly Database db;
        private readonly IFluentEmailFactory emailSvc;

        private async Task<Soldier> CurrentUser() => await UserService.FindAsync(db, User);

        public HomeController(Database db, IFluentEmailFactory emailSvc)
        {
            this.db = db;
            this.emailSvc = emailSvc;
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

        public IActionResult RequestAccess()
        {
            return View(new RequestAccessModel
            {
                Name = User.FindFirst("name")?.Value,
                Email = User.FindFirst("email")?.Value,
                Units = from unit in db.Units
                        orderby unit.Name
                        select new SelectListItem
                        {
                            Text = unit.Name,
                            Value = $"{unit.Id}"
                        }
            });
        }

        [HttpPost]
        public async Task<IActionResult> RequestAccess(RequestAccessModel model)
        {
            // Process request, send email to admin with info

            UserService.RequestAccess(emailSvc, model);

            // Redirect to thank you etc. etc.

            TempData["Message"] = "Your request has been submitted. Thank you!";

            return RedirectToAction(nameof(Index));
        }

        [Route("~/Logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();

            return RedirectToAction(nameof(Login));
        }

        [Route("~/Error"), AllowAnonymous]
        public IActionResult Error()
        {
            return View();
        }

        public async Task<IActionResult> UnknownRoute(String url)
        {
            // Try to find an embed matching the given route

            var embed = await db.Embeds.SingleOrDefaultAsync(_ => _.Route == url);

            if (embed != null)
            {
                // If found, return that

                return View("Embed", embed);
            }

            // If not, return 404

            return NotFound();
        }
    }
}