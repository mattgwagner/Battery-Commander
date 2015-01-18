using BatteryCommander.Common;
using BatteryCommander.Common.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BatteryCommander.Web.Controllers
{
    public class SoldierController : BaseController
    {
        private readonly DataContext _db;

        public SoldierController(UserManager<AppUser, int> userManager, DataContext db)
            : base(userManager)
        {
            _db = db;
        }

        public ActionResult Index()
        {
            return View();
        }
    }
}