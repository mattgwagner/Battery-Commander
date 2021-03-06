﻿using BatteryCommander.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace BatteryCommander.Web.Controllers.API
{
    public class WeaponsController : ApiController
    {
        public WeaponsController(Database db) : base(db)
        {
            // Nothing to do here
        }

        [HttpGet]
        public async Task<IActionResult> Get(int? unit)
        {
            // GET: api/weapons

            return Ok(
                await db
                .Weapons
                .Where(weapon => !unit.HasValue || weapon.UnitId == unit)
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
                .ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Weapon weapon)
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