using BatteryCommander.Web.Models;
using BatteryCommander.Web.Models.Settings;
using BatteryCommander.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static BatteryCommander.Web.Models.Reports.Report;

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

        [HttpGet("[controller]/{unitId}/reports")]
        public async Task<IEnumerable<ReportSettings>> Reports(int unitId)
        {
            return
                await db
                .Units
                .Where(unit => unit.Id == unitId)
                .SelectMany(unit => unit.ReportSettings)
                .ToListAsync();
        }

        [HttpPost("[controller]/{unitId}/reports")]
        public async Task<IActionResult> AddReport([FromRoute]int unitId, [FromBody]ReportSettings settings)
        {
            var unit = await UnitService.Get(db, unitId);

            var model = unit.ReportSettings;

            model.Add(settings);

            unit.ReportSettings = model;

            await db.SaveChangesAsync();

            return Accepted();
        }

        [HttpDelete("[controller]/{unitId}/reports/{reportType}")]
        public async Task<IActionResult> AddReport([FromRoute]int unitId, [FromRoute]ReportType type)
        {
            var unit = await UnitService.Get(db, unitId);

            var to_remove =
                unit
                .ReportSettings
                .Where(report => report.Type == type)
                .FirstOrDefault();

            if (to_remove != null)
            {
                unit.ReportSettings.Remove(to_remove);

                await db.SaveChangesAsync();
            }

            return Ok();
        }
    }
}