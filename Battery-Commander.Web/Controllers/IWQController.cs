using BatteryCommander.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using BatteryCommander.Web.Services;

namespace BatteryCommander.Web.Controllers
{
    [Authorize, ApiExplorerSettings(IgnoreApi = true)]
    public class IWQController : Controller
    {
        private readonly Database db;

        public IWQController(Database db)
        {
            this.db = db;
        }

        // List status for all soldiers by - filter by unit

        // Bulk update qual status/date

        public async Task<IActionResult> Index(SoldierService.Query query)
        {
            return View("List", new IwqListViewModel
            {
                Rows = 
                    (await SoldierService.Filter(db, query))
                    .Select(soldier => new IwqListViewModel.Row
                    {
                        Soldier = soldier,
                        SoldierId = soldier.Id,
                        IwqQualificationDate = soldier.IwqQualificationDate
                    })
                    .ToList()
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(IwqListViewModel model)
        {
            // For each DTO posted, update the soldier info

            foreach (var dto in model.Rows)
            {
                var soldier =
                    await db
                    .Soldiers
                    .Where(_ => _.Id == dto.SoldierId)
                    .SingleOrDefaultAsync();

                soldier.IwqQualificationDate = dto.IwqQualificationDate;
            }

            await db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public class IwqListViewModel
        {
            public IList<Row> Rows { get; set; }

            public class Row
            {
                public Soldier Soldier { get; set; }

                public int SoldierId { get; set; }

                [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
                public DateTime? IwqQualificationDate { get; set; }
            }
        }
    }
}