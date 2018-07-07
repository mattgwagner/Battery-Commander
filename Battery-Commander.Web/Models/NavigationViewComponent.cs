using BatteryCommander.Web.Controllers;
using BatteryCommander.Web.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
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
                return
                    db
                    .Embeds
                    .Where(embed => !embed.UnitId.HasValue || Soldier == null || embed.UnitId == Soldier.UnitId)
                    .ToList();
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