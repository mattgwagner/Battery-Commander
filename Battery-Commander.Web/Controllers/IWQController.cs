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
    [Authorize]
    public class IWQController : Controller
    {
        private readonly Database db;

        public IWQController(Database db)
        {
            this.db = db;
        }

        // List status for all soldiers by - filter by unit

        // Bulk update qual status/date

        public async Task<IActionResult> Index(SoldierSearchService.Query query)
        {
            var soldiers = await SoldierSearchService.Filter(db, query);

            return View("List", soldiers);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(IWQListViewModel model)
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