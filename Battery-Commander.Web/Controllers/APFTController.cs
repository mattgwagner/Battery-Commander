using BatteryCommander.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BatteryCommander.Web.Controllers
{
    [Authorize]
    public class APFTController : Controller
    {
        private readonly Database db;

        public APFTController(Database db)
        {
            this.db = db;
        }
    }
}