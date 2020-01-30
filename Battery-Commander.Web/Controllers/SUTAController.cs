using System;
using System.Threading.Tasks;
using BatteryCommander.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BatteryCommander.Web.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class SUTAController : Controller
    {
        private readonly Database db;

        public SUTAController(Database db)
        {
            this.db = db;
        }

        [HttpGet(""), AllowAnonymous]
        public async Task<IActionResult> New()
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        public async Task<IActionResult> New(SUTA model)
        {
            // Validate

            // Persist

            // Redirect with confirmation message

            return RedirectToAction(nameof(Details), new { id = 1 });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Details(int id)
        {
            throw new NotImplementedException();
        }

        [HttpGet("~/Units/{id}/SUTAs")]
        public async Task<IActionResult> ByUnit(int id)
        {
            // Retrieve SUTA requests by unit

            throw new NotImplementedException();
        }        

        [HttpGet("{id}/Edit")]
        public async Task<IActionResult> Edit(int id)
        {
            throw new NotImplementedException();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Transition(int id, Evaluation.Trigger trigger)
        {
            //var evaluation = await Evaluations.SingleOrDefaultAsync(_ => _.Id == id);

            //evaluation.Transition(trigger);

            //evaluation.Events.Add(new Evaluation.Event
            //{
            //    Author = await GetDisplayName(),
            //    Message = trigger.DisplayName()
            //});

            //await db.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id });
        }
    }
}
