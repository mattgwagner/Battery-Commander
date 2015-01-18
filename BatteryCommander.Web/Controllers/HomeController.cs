using BatteryCommander.Common.Models;
using Microsoft.AspNet.Identity;
using System.Web.Mvc;

namespace BatteryCommander.Web.Controllers
{
    public class HomeController : BaseController
    {
        public HomeController(UserManager<AppUser, int> userManager)
            : base(userManager)
        {
        }

        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }
    }
}