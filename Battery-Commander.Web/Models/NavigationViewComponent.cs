using BatteryCommander.Web.Controllers;
using BatteryCommander.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;

namespace BatteryCommander.Web.Models
{
    public class NavigationViewComponent : ViewComponent
    {
        private readonly IMemoryCache cache;
        private readonly Database db;

        public Soldier Soldier { get; private set; }

        public Boolean ShowNavigation => !String.Equals(nameof(HomeController.PrivacyAct), RouteData.Values["action"]);

        public NavigationViewComponent(IMemoryCache cache, Database db)
        {
            this.cache = cache;
            this.db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            Soldier = await UserService.FindAsync(db, this.UserClaimsPrincipal, cache);

            return View(this);
        }
    }
}