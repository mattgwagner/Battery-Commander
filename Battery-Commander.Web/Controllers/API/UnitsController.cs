using BatteryCommander.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BatteryCommander.Web.Controllers.API
{
    public class UnitsController : ApiController
    {
        public UnitsController(Database db) : base(db)
        {
            // Nothing to do here
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