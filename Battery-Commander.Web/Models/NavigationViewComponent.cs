using System;
using System.Threading.Tasks;
using BatteryCommander.Web.Controllers;
using BatteryCommander.Web.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BatteryCommander.Web.Models
{
    public class NavigationViewComponent : ViewComponent
    {
        private readonly IMediator dispatcher;

        public Soldier Soldier { get; private set; }

        public Boolean ShowNavigation => Soldier != null && !String.Equals(nameof(HomeController.PrivacyAct), RouteData.Values["action"]);

        public NavigationViewComponent(IMediator dispatcher)
        {
            this.dispatcher = dispatcher;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            Soldier = await dispatcher.Send(new GetCurrentUser { });

            return View(this);
        }
    }
}