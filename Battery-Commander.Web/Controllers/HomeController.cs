﻿using BatteryCommander.Web.Models;
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

            foreach (var unit in db.Units.Where(unit => !unit.IgnoreForReports))
            {
                var soldiers =
                    await db
                    .Soldiers
                    .Include(s => s.ABCPs)
                    .Include(s => s.APFTs)
                    .Where(s => s.UnitId == unit.Id)
                    .ToListAsync();

                if (soldiers.Any())
                {
                    model.Add(new UnitStatsViewModel
                    {
                        Unit = unit,
                        APFT = new UnitStatsViewModel.Stat
                        {
                            Assigned = soldiers.Count,
                            Passed = soldiers.Where(soldier => soldier.ApftStatus == Soldier.EventStatus.Passed).Count(),
                            Failed = soldiers.Where(soldier => soldier.ApftStatus == Soldier.EventStatus.Failed).Count(),
                            NotTested = soldiers.Where(soldier => soldier.ApftStatus == Soldier.EventStatus.NotTested).Count()
                        },
                        ABCP = new UnitStatsViewModel.Stat
                        {
                            Assigned = soldiers.Count,
                            Passed = soldiers.Where(soldier => soldier.AbcpStatus == Soldier.EventStatus.Passed).Count(),
                            Failed = soldiers.Where(soldier => soldier.AbcpStatus == Soldier.EventStatus.Failed).Count(),
                            NotTested = soldiers.Where(soldier => soldier.AbcpStatus == Soldier.EventStatus.NotTested).Count()
                        },
                        SSD =
                            RankExtensions
                            .All()
                            .Where(rank => rank.IsEnlisted() || rank.IsNCO())
                            .Select(rank => new UnitStatsViewModel.SSDStat
                            {
                                Rank = rank,
                                Assigned = soldiers.Where(soldier => soldier.Rank == rank).Count(),
                                Completed = soldiers.Where(soldier => soldier.Rank == rank).Where(soldier => soldier.SSDStatus.CurrentProgress >= Decimal.One).Count()
                            })
                            .ToList()
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

        public IActionResult Error()
        {
            return View();
        }
    }
}