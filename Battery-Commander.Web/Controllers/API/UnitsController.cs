using BatteryCommander.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BatteryCommander.Web.Controllers.API
{
    [Route("api/[controller]"), Authorize]
    public class UnitsController : Controller
    {
        private readonly Database db;

        public UnitsController(Database db)
        {
            this.db = db;
        }

        [HttpGet]
        public async Task<IEnumerable<dynamic>> Get()
        {
            // GET: api/units

            return
                await db
                .Units
                .Select(unit => new
                {
                    unit.Id,
                    unit.Name,
                    unit.UIC
                })
                .ToListAsync();
        }
    }
}