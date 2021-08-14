using BatteryCommander.Web.Models;
using BatteryCommander.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BatteryCommander.Web.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class SupervisorsController : Controller
    {
        private readonly Database db;

        public SupervisorsController(Database db)
        {
            this.db = db;
        }

        // List status for all soldiers by - filter by unit

        // Bulk update supervisors

        public async Task<IActionResult> Index(SoldierService.Query query)
        {
            return View("List", new SupervisorListModel
            {
                Unit = query.Unit,

                Soldiers = await SoldierService.GetDropDownList(db),

                Rows =
                    (await SoldierService.Filter(db, query))
                    .OrderByDescending(soldier => soldier.Rank)
                    .ThenBy(soldier => soldier.LastName)
                    .Select(soldier => new SupervisorListModel.Row
                    {
                        Soldier = soldier,
                        SoldierId = soldier.Id,
                        SupervisorId = soldier.SupervisorId
                    })
                    .ToList()
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(SupervisorListModel model)
        {
            // For each DTO posted, update the soldier info

            foreach (var dto in model.Rows)
            {
                var soldier =
                    await db
                    .Soldiers
                    .Where(_ => _.Id == dto.SoldierId)
                    .SingleOrDefaultAsync();

                soldier.SupervisorId = dto.SupervisorId;
            }

            await db.SaveChangesAsync();

            return RedirectToRoute("Unit.Soldiers", new { unitId = model.Unit });

            return RedirectToAction(nameof(Index));
        }

        public class SupervisorListModel
        {
            public int? Unit { get; set; }

            public IList<Row> Rows { get; set; }

            public IEnumerable<SelectListItem> Soldiers { get; set; } = Enumerable.Empty<SelectListItem>();

            public class Row
            {
                public Soldier Soldier { get; set; }

                public int SoldierId { get; set; }

                public int? SupervisorId { get; set; }
            }
        }
    }
}