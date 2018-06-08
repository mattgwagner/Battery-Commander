using BatteryCommander.Web.Controllers;
using BatteryCommander.Web.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace BatteryCommander.Web.Models
{
    public class NavigationViewComponent : ViewComponent
    {
        private readonly Database db;

        public Soldier Soldier { get; private set; }

        public Boolean ShowNavigation => !String.Equals(nameof(HomeController.PrivacyAct), RouteData.Values["action"]);

        // TODO Fix this to actually check a feature flag

        public Boolean ShowSUTATracker => UserClaimsPrincipal?.Identity.Name == "mattgwagner@gmail.com";

        public Boolean ShowPOTracker => UserClaimsPrincipal?.Identity.Name == "mattgwagner@gmail.com";

        public NavigationViewComponent(Database db)
        {
            this.db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            Soldier = await UserService.FindAsync(db, UserClaimsPrincipal);

            return View(this);
        }
    }
}