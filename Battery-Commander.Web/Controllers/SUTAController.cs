using BatteryCommander.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BatteryCommander.Web.Controllers
{
    public class SUTAController : Controller
    {
        private readonly AirTableService airtable;

        public SUTAController(AirTableService airtable)
        {
            this.airtable = airtable;
        }

        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult Create()
        {
            return View();
        }
    }
}