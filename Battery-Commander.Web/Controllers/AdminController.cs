using BatteryCommander.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;

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
            byte[] data;

            using (var stream = new FileStream($@"logs\{DateTime.Today:yyyyMMdd}.log", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var reader = new StreamReader(stream))
            {
                data = System.Text.Encoding.Default.GetBytes(reader.ReadToEnd());
            }

            return File(data, "text/plain");
        }
    }
}