using BatteryCommander.Web.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BatteryCommander.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly Database db;

        public HomeController(Database db)
        {
            this.db = db;
        }

        public async Task<IActionResult> Index()
        {
            var model = new List<UnitStatsViewModel>();

            foreach (var unit in db.Units)
            {
                var soldiers =
                    await db
                    .Soldiers
                    .Include(s => s.ABCPs)
                    .Include(s => s.APFTs)
                    .Where(s => s.UnitId == unit.Id)
                    .Select(s => new
                    {
                        Soldier = s,
                        APFT = s.APFTs.OrderByDescending(_ => _.Date).FirstOrDefault(),
                        ABCP = s.ABCPs.OrderByDescending(_ => _.Date).FirstOrDefault()
                    })
                    .ToListAsync();

                if (soldiers.Any())
                {
                    model.Add(new UnitStatsViewModel
                    {
                        Unit = unit,
                        APFT = new UnitStatsViewModel.Stat
                        {
                            Assigned = soldiers.Count,
                            Passed = soldiers.Where(_ => _.APFT?.IsPassing == true).Count(),
                            Failed = soldiers.Where(_ => _.APFT?.IsPassing == false).Count(),
                            NotTested = soldiers.Where(_ => _.APFT == null).Count()
                        },
                        ABCP = new UnitStatsViewModel.Stat
                        {
                            Assigned = soldiers.Count,
                            Passed = soldiers.Where(_ => _.ABCP?.IsPassing == true).Count(),
                            Failed = soldiers.Where(_ => _.ABCP?.IsPassing == false).Count(),
                            NotTested = soldiers.Where(_ => _.ABCP == null).Count()
                        }
                    });
                }
            }

            return View(model);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        [AllowAnonymous]
        public IActionResult Login(String returnUrl = "/")
        {
            return new ChallengeResult("Auth0", new AuthenticationProperties { RedirectUri = returnUrl });
        }

        [Route("~/Logout")]
        public async Task Logout()
        {
            await HttpContext.Authentication.SignOutAsync("Auth0", new AuthenticationProperties
            {
                RedirectUri = Url.Action(nameof(Index))
            });

            await HttpContext.Authentication.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        public IActionResult Backup()
        {
            var data = System.IO.File.ReadAllBytes("Data.db");

            var mimeType = "application/octet-stream";

            return File(data, mimeType);
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}