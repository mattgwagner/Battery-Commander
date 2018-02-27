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
    public class WeaponsController : Controller
    {
        private readonly Database db;

        public WeaponsController(Database db)
        {
            this.db = db;
        }

        [HttpGet]
        public async Task<IEnumerable<dynamic>> Get()
        {
            // GET: api/weapons

            return
                await db
                .Weapons
                .Select(_ => new
                {
                    _.Id,
                    _.OpticSerial,
                    _.OpticType,
                    _.Serial,
                    _.StockNumber,
                    _.Type,
                    _.UnitId
                })
                .ToListAsync();
        }

        [HttpPost]
        public async Task<IActionResult> Post(Weapon weapon)
        {
            await db.Weapons.AddAsync(weapon);

            await db.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var weapon = await db.Weapons.FindAsync(id);

            db.Weapons.Remove(weapon);

            await db.SaveChangesAsync();

            return Ok();
        }
    }
}