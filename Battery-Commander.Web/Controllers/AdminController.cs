using BatteryCommander.Web.Models;
using FluentScheduler;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BatteryCommander.Web.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class AdminController : Controller
    {
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
                .Include(soldier => soldier.Unit)
                .Where(soldier => soldier.CanLogin)
                .Where(soldier => !String.IsNullOrWhiteSpace(soldier.CivilianEmail))
                .Select(soldier => new
                {
                    Uri = Url.RouteUrl("Soldier.Details", new { soldier.Id }, Request.Scheme),
                    Unit = soldier.Unit.Name,
                    soldier.FirstName,
                    soldier.LastName,
                    soldier.CivilianEmail
                })
                .ToListAsync();

            return Json(soldiers_with_access);
        }
    }
}
