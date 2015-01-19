using BatteryCommander.Common;
using BatteryCommander.Common.Models;
using BatteryCommander.Web.Models;
using Microsoft.AspNet.Identity;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Linq;

namespace BatteryCommander.Web.Controllers
{
    public class AlertController : BaseController
    {
        private readonly DataContext _db;

        public AlertController(UserManager<AppUser, int> userManager, DataContext db)
            : base(userManager)
        {
            _db = db;
        }

        [Route("~/Alerts")]
        public async Task<ActionResult> List()
        {
            var alerts =
                await _db
                .Alerts
                .OrderByDescending(a => a.SendDateUtc)
                .ToListAsync();

            return View(alerts);
        }

        public async Task<ActionResult> Send()
        {
            var model = new SendAlertModel
            {

            };

            // TODO Show groups, soldiers, positions

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Send(SendAlertModel model)
        {
            if (!ModelState.IsValid) return View(model);

            // TODO Send alert message to the selected soldiers/groups

            return RedirectToAction("List");
        }
    }
}