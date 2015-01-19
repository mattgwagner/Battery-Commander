using BatteryCommander.Common;
using BatteryCommander.Common.Models;
using Microsoft.AspNet.Identity;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BatteryCommander.Web.Controllers
{
    public class BattleRosterController : BaseController
    {
        private readonly DataContext _db;

        public BattleRosterController(UserManager<AppUser, int> userManager, DataContext db)
            : base(userManager)
        {
            _db = db;
        }

        public async Task<ActionResult> Index()
        {
            // TODO Which qualifications to show?

            // TODO How to order soldiers?

            return View();
        }
    }
}