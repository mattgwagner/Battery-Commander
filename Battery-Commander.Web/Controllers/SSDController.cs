using BatteryCommander.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace BatteryCommander.Web.Controllers
{
    [Authorize]
    public class SSDController : Controller
    {
        private readonly Database db;

        public SSDController(Database db)
        {
            this.db = db;
        }

        public async Task<IActionResult> Index(int? unit)
        {
            var model =
                await db
                .Soldiers
                .Include(s => s.SSDSnapshots)
                .Where(s => !unit.HasValue || s.UnitId == unit)
                .OrderBy(s => s.LastName)
                .ThenBy(s => s.FirstName)
                .ToListAsync();

            return Json(from s in model
                        select new
                        {
                            s.LastName,
                            s.SSDStatus
                        }
                        );
        }
    }
}