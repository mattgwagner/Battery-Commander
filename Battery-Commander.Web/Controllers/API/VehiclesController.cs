using BatteryCommander.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BatteryCommander.Web.Controllers.API
{
    public class VehiclesController : ApiController
    {
        public VehiclesController(Database db) : base(db)
        {
            // Nothing to do here
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