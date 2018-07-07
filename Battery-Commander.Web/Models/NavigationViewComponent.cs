using BatteryCommander.Web.Controllers;
using BatteryCommander.Web.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BatteryCommander.Web.Models
{
    public class NavigationViewComponent : ViewComponent
    {
        private readonly Database db;

        public Soldier Soldier { get; private set; }

        public Boolean ShowNavigation => !String.Equals(nameof(HomeController.PrivacyAct), RouteData.Values["action"]);

        public IEnumerable<Embed> NavItems
        {
            get
            {
                // TODO Only for nav items
                // TODO Only for the logged in unit

                yield return new Embed { Name = "PO Tracker" };

                yield return new Embed { Name = "SUTA" };

                //return
                //    db
                //    .Embeds
                //    .ToList();
            }
        }

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