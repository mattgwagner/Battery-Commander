using BatteryCommander.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BatteryCommander.Web.Controllers
{
    [Authorize]
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

        public async Task<IActionResult> Scrub()
        {
            // Go through each soldier and fix casing on their name

            foreach(var soldier in db.Soldiers)
            {
                soldier.FirstName = soldier.FirstName.ToTitleCase();
                soldier.MiddleName = soldier.MiddleName.ToTitleCase();
                soldier.LastName = soldier.LastName.ToTitleCase();
            }

            await db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Backup()
        {
            var data = System.IO.File.ReadAllBytes("Data.db");

            var mimeType = "application/octet-stream";

            return File(data, mimeType);
        }
    }
}