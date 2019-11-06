using BatteryCommander.Web.Models;
using FluentScheduler;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace BatteryCommander.Web.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class AdminController : Controller
    {
        // Admin Tasks:

        // Add/Remove Users

        // Backup SQLite Db

        // Scrub Soldier Data

        private readonly Database db;

        public AdminController(Database db)
        {
            this.db = db;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Backup()
        {
            var data = System.IO.File.ReadAllBytes("Data.db");

            var mimeType = "application/octet-stream";

            return File(data, mimeType);
        }

        public IActionResult Jobs()
        {
            return View(JobManager.AllSchedules);
        }

        public async Task<IActionResult> Users()
        {
            var soldiers_with_access =
                await db
                .Soldiers
                .Where(soldier => soldier.CanLogin)
                .Select(soldier => new
                {
                    soldier.FirstName,
                    soldier.LastName,
                    soldier.CivilianEmail
                })
                .ToListAsync();

            return Json(soldiers_with_access);
        }
    }
}