using BatteryCommander.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BatteryCommander.Web.Controllers
{
    [Authorize]
    public class SoldierController : Controller
    {
        private readonly Database db;

        public SoldierController(Database db)
        {
            this.db = db;
        }
    }
}