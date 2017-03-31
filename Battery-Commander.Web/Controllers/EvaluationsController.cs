using BatteryCommander.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BatteryCommander.Web.Controllers
{
    [Authorize]
    public class EvaluationsController : Controller
    {
        private readonly Database db;

        public EvaluationsController(Database db)
        {
            this.db = db;
        }
    }
}