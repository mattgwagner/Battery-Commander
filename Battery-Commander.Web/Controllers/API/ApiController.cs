using BatteryCommander.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BatteryCommander.Web.Controllers.API
{
    [Route("api/[controller]"), Authorize(AuthenticationSchemes = "Bearer,Cookies")]
    public class ApiController : Controller
    {
        protected readonly Database db;

        public ApiController(Database db)
        {
            this.db = db;
        }
    }
}