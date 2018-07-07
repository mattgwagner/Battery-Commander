using BatteryCommander.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BatteryCommander.Web.Controllers.API
{
    public class EmbedsController : ApiController
    {
        public EmbedsController(Database db) : base(db)
        {
            // Nothing to do here
        }

        [HttpGet]
        public async Task<IEnumerable<dynamic>> List()
        {
            return
                await db
                .Embeds
                .Select(embed => new
                {
                    embed.Id,
                    embed.UnitId,
                    embed.Name,
                    embed.Route,
                    embed.Source
                })
                .ToListAsync();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody]Embed embed)
        {
            db.Embeds.Add(embed);

            await db.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var embed = await db.Embeds.SingleOrDefaultAsync(_ => _.Id == id);

            db.Embeds.Remove(embed);

            await db.SaveChangesAsync();

            return Ok();
        }
    }
}