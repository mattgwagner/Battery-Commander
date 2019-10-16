using BatteryCommander.Web.Models;
using BatteryCommander.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
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

        public async Task<IActionResult> Index(SoldierService.Query query, String redirectUrl)
        {
            return View("List", new StatusListModel
            {
                RedirectUrl = String.IsNullOrWhiteSpace(redirectUrl) ? $"{Request.Headers["Referrer"]}" : redirectUrl,
                Rows =
                    (await SoldierService.Filter(db, query))
                    .Select(soldier => new StatusListModel.Row
                    {
                        Soldier = soldier,
                        SoldierId = soldier.Id,
                        Status = soldier.Status
                    })
                    .ToList()
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(StatusListModel model)
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

            if(!String.IsNullOrWhiteSpace(model.RedirectUrl))
            {
                return Redirect(model.RedirectUrl);
            }

            return RedirectToAction(nameof(Index));
        }

        public class StatusListModel
        {
            public IList<Row> Rows { get; set; }
            public string RedirectUrl { get; internal set; }

            public class Row
            {
                public Soldier Soldier { get; set; }

                public int SoldierId { get; set; }

                public Soldier.SoldierStatus Status { get; set; } = Soldier.SoldierStatus.Unknown;
            }
        }
    }
}