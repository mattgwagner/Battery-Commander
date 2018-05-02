using BatteryCommander.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BatteryCommander.Web.Controllers
{
    [AllowAnonymous]
    public class SUTAController : Controller
    {
        private readonly AirTableService airtable;

        public SUTAController(AirTableService airtable)
        {
            this.airtable = airtable;
        }

        public IActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction(nameof(Create));
            }

            return View();
        }

        public IActionResult Create()
        {
            return View();
        }
    }
}