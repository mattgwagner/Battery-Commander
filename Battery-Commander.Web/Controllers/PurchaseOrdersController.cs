using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BatteryCommander.Web.Controllers
{
    public class PurchaseOrdersController : Controller
    {
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