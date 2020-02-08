using System.Linq;
using System.Threading.Tasks;
using BatteryCommander.Web.Commands;
using BatteryCommander.Web.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BatteryCommander.Web.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true), Route("[controller]")]
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
            ViewBag.Soldiers =
                (await dispatcher.Send(new GetSoldiers { }))
                .OrderBy(soldier => soldier.LastName)
                .ThenBy(soldier => soldier.FirstName)
                .Select(soldier => new SelectListItem
                {
                    Text = $"{soldier.RankHumanized} {soldier.LastName} {soldier.FirstName}",
                    Value = $"{soldier.Id}"
                });

            return View();
        }

        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> New([FromForm]AddSUTARequest request)
        {
            var id = await dispatcher.Send(request);

            ViewBag.Id = id;

            return await New();
        }

        [HttpGet("{Id}"), AllowAnonymous]
        public async Task<IActionResult> Details(GetSUTARequest request) => View(await dispatcher.Send(request));

        [HttpGet("~/Units/{Unit}/SUTA")]
        public async Task<IActionResult> ByUnit(GetSUTARequests request) => View("List", await dispatcher.Send(request));

        [HttpGet("{Id}/Edit")]
        public async Task<IActionResult> Edit(GetSUTARequest request) => View(UpdateSUTARequest.Build(await dispatcher.Send(request)));

        [HttpPost("{Id}")]
        public async Task<IActionResult> Edit(UpdateSUTARequest request)
        {
            await dispatcher.Send(request);

            return RedirectToAction(nameof(Details), new { request.Id });
        }

        [HttpPost("{Id}/[action]")]
        public async Task<IActionResult> Delete(DeleteSUTARequest request)
        {
            await dispatcher.Send(request);

            return RedirectToAction(nameof(New));
        }

        [HttpPost("[action]"), ValidateAntiForgeryToken]
        public async Task<IActionResult> Sign(SignSUTARequest request)
        {
            await dispatcher.Send(request);

            return RedirectToAction(nameof(Details), new { request.Id });
        }

        [HttpPost("[action]"), ValidateAntiForgeryToken]
        public async Task<IActionResult> Comment(AddSUTAComment request)
        {
            await dispatcher.Send(request);

            return RedirectToAction(nameof(Details), new { request.Id });
        }
    }
}