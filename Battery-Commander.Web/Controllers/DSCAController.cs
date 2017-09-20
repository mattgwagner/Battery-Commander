using BatteryCommander.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BatteryCommander.Web.Controllers
{
    [Authorize]
    public class DSCAController : Controller
    {
        private readonly Database db;

        public DSCAController(Database db)
        {
            this.db = db;
        }

        // List status for all soldiers by - filter by unit

        // Bulk update qual status/date

        public async Task<IActionResult> Index()
        {
            return View("List", new DSCAListViewModel
            {
                Rows =
                    (await db
                    .Soldiers
                    .Include(soldier => soldier.Unit)
                    .Where(soldier => !soldier.Unit.IgnoreForReports)
                    .OrderBy(soldier => soldier.LastName)
                    .ThenBy(soldier => soldier.FirstName)
                    .ToListAsync())
                    .Select(soldier => new DSCAListViewModel.Row
                    {
                        Soldier = soldier,
                        SoldierId = soldier.Id,
                        DscaQualificationDate = soldier.DscaQualificationDate
                    })
                    .ToList()
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(DSCAListViewModel model)
        {
            // For each DTO posted, update the soldier info

            foreach (var dto in model.Rows)
            {
                var soldier =
                    await db
                    .Soldiers
                    .Where(_ => _.Id == dto.SoldierId)
                    .SingleOrDefaultAsync();

                // Update DSCA info

                soldier.DscaQualificationDate = dto.DscaQualificationDate;
            }

            await db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public class DSCAListViewModel
        {
            public IList<Row> Rows { get; set; }

            public class Row
            {
                public Soldier Soldier { get; set; }

                public int SoldierId { get; set; }

                [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
                public DateTime? DscaQualificationDate { get; set; }
            }
        }
    }
}