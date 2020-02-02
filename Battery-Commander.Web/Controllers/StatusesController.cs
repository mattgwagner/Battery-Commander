using BatteryCommander.Web.Models;
using BatteryCommander.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BatteryCommander.Web.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class StatusesController : Controller
    {
        private readonly Database db;

        public StatusesController(Database db)
        {
            this.db = db;
        }

        // List status for all soldiers by - filter by unit

        // Bulk update statuses

        [HttpGet("~/Units/{Unit}/PERSTAT")]
        public async Task<IActionResult> Index(int Unit)
        {
            return View("List", new StatusListModel
            {
                Unit = Unit,
                Rows =
                    (await SoldierService.Filter(db, new SoldierService.Query
                    {
                        Unit = Unit
                    }))
                    .Select(soldier => new StatusListModel.Row
                    {
                        Soldier = soldier,
                        SoldierId = soldier.Id,
                        Status = soldier.Status
                    })
                    .ToList()
            });
        }

        [HttpPost("~/Unis/{Unit}/PERSTAT"), ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(int Unit, StatusListModel model)
        {
            // For each DTO posted, update the soldier info

            foreach (var dto in model.Rows)
            {
                var soldier =
                    await db
                    .Soldiers
                    .Where(_ => _.Id == dto.SoldierId)
                    .SingleOrDefaultAsync();

                soldier.Status = dto.Status;
            }

            await db.SaveChangesAsync();

            return RedirectToAction("ForUnit", "Soldiers", new { unitId = Unit });
        }

        public class StatusListModel
        {
            public IList<Row> Rows { get; set; }

            public int Unit { get; set; }

            public class Row
            {
                public Soldier Soldier { get; set; }

                public int SoldierId { get; set; }

                public Soldier.SoldierStatus Status { get; set; } = Soldier.SoldierStatus.Unknown;
            }
        }
    }
}