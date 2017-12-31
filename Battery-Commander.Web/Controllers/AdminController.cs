using BatteryCommander.Web.Models;
using BatteryCommander.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace BatteryCommander.Web.Controllers
{
    [Authorize, ApiExplorerSettings(IgnoreApi = true)]
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

        public IActionResult Logs()
        {
            var data = System.IO.File.ReadAllBytes($@"logs\{DateTime.Today:yyyyMMdd}.log");

            var mimeType = "text/plain";

            return File(data, mimeType);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Calendar(int unitId, String apiKey)
        {
            var data = await CalendarService.Generate(this.db, unitId, apiKey);

            return File(data, "text/calendar");
        }
    }
}