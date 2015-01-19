using BatteryCommander.Common;
using BatteryCommander.Common.Models;
using Microsoft.AspNet.Identity;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BatteryCommander.Web.Controllers
{
    public class WorkItemController : BaseController
    {
        private readonly DataContext _db;

        public WorkItemController(UserManager<AppUser, int> userManager, DataContext db)
            : base(userManager)
        {
            _db = db;
        }

        [Route("WorkItems")]
        public async Task<ActionResult> List()
        {
            var workItems =
                await _db
                .WorkItems
                .Include(w => w.Assignee)
                .ToListAsync();

            return View(workItems);
        }

        [Route("WorkItem/{workItemId}")]
        public async Task<ActionResult> View(int workItemId)
        {
            var workItem =
                await _db
                .WorkItems
                .Include(w => w.Assignee)
                .SingleOrDefaultAsync(w => w.Id == workItemId);

            return View(workItem);
        }
    }
}