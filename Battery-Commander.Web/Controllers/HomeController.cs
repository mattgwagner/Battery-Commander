using System;
using System.Linq;
using System.Threading.Tasks;
using BatteryCommander.Web.Models;
using BatteryCommander.Web.Queries;
using BatteryCommander.Web.Services;
using FluentEmail.Core;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BatteryCommander.Web.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class HomeController : Controller
    {
        private readonly Database db;
        private readonly IFluentEmailFactory emailSvc;
		private readonly IMediator dispatcher;

        public HomeController(Database db, IFluentEmailFactory emailSvc, IMediator dispatcher)
        {
            this.db = db;
            this.emailSvc = emailSvc;
			this.dispatcher = dispatcher;
		}

        public ActionResult PrivacyAct()
        {
            return View();
        }

        [AllowAnonymous]
        public async Task Login(String ReturnUrl = "/")
        {
            await HttpContext.ChallengeAsync("Auth0", new AuthenticationProperties
            {
                RedirectUri = ReturnUrl?.Equals("/") == true ? Url.Action(nameof(PrivacyAct)) : ReturnUrl
            });
        }

        [Route("~/RequestAccess")]
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

        [Route("~/RequestAccess"), HttpPost]
        public async Task<IActionResult> RequestAccess(RequestAccessModel model)
        {
            // Process request, send email to admin with info

            await UserService.RequestAccess(emailSvc, model);

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
            // If not, return 404

            return NotFound();
        }
    }
}