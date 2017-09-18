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
            var soldiers =
                await db
                .Soldiers
                .Include(soldier => soldier.Unit)
                .ToListAsync();

            return View("List", soldiers);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(IEnumerable<DTO> model)
        {
            // For each DTO posted, update the soldier info

            foreach (var dto in model)
            {
                var soldier =
                    await db
                    .Soldiers
                    .Where(_ => _.Id == dto.SoldierId)
                    .SingleOrDefaultAsync();

                // Update DSCA info

                soldier.DscaQualificationDate = dto.QualificationDate;
            }

            await db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public class DTO
        {
            public int SoldierId { get; set; }

            [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
            public DateTime? QualificationDate { get; set; }
        }
    }
}