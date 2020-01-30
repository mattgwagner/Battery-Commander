using System.Threading.Tasks;
using BatteryCommander.Web.Commands;
using BatteryCommander.Web.Models;
using BatteryCommander.Web.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BatteryCommander.Web.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class SUTAController : Controller
    {
        private readonly IMediator dispatcher;

        public SUTAController(IMediator dispatcher)
        {
            this.dispatcher = dispatcher;
        }

        [HttpGet(""), AllowAnonymous]
        public async Task<IActionResult> New()
        {
            // TODO Populate dropdown lists

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Save([FromForm]AddSUTARequest request)
        {
            var id = await dispatcher.Send(request);

            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpGet("{Id}")]
        public async Task<IActionResult> Details(GetSUTARequest request) => View(await dispatcher.Send(request));

        [HttpGet("~/Units/{Unit}/SUTA")]
        public async Task<IActionResult> ByUnit(GetSUTARequests request) => View(await dispatcher.Send(request));

        [HttpGet("{Id}/Edit")]
        public async Task<IActionResult> Edit(GetSUTARequest request) => View(await dispatcher.Send(request));

        [HttpPost("{Id}")]
        public async Task<IActionResult> Edit(UpdateSUTARequest request)
        {
            await dispatcher.Send(request);

            return RedirectToAction(nameof(Details), new { request.Id });
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

            // TODO Add an event, alert the people on the request

            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Comment(int id, string comment)
        {
            // TODO Add a comment, alert the people on the request

            return RedirectToAction(nameof(Details), new { id });
        }
    }
}