using BatteryCommander.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BatteryCommander.Web.Controllers
{
    public class ACFTController : Controller
    {
        private readonly Database db;

        public ACFTController(Database db)
        {
            this.db = db;
        }

        public async Task<IActionResult> Index()
        {
            return Content("Coming soon!");
        }
    }
}
