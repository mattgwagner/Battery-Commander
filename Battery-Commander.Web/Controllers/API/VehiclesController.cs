using BatteryCommander.Web.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BatteryCommander.Web.Controllers.API
{
    [Route("api/[controller]"), Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class VehiclesController : Controller
    {
        private readonly Database db;

        public VehiclesController(Database db)
        {
            this.db = db;
        }

        [HttpGet]
        public async Task<IEnumerable<dynamic>> Get()
        {
            // GET: api/vehicles

            return
                await db
                .Vehicles
                .Select(_ => new
                {
                    _.Id,
                    _.UnitId,
                    _.Bumper,
                    _.Registration,
                    _.Nomenclature,
                    _.Notes,
                    _.Serial,
                    _.Status,
                    _.Type,
                    _.HasFuelCard,
                    _.HasJBCP,
                    _.HasTowBar
                })
                .ToListAsync();
        }
    }
}