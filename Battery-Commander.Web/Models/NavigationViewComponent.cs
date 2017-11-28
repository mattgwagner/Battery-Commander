using BatteryCommander.Web.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BatteryCommander.Web.Models
{
    public class NavigationViewComponent : ViewComponent
    {
        private readonly Database db;

        public NavigationViewComponent(Database db)
        {
            this.db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync(ClaimsPrincipal user)
        {
            return View(new ViewModel { Soldier = await UserService.FindAsync(db, user) });
        }

        public class ViewModel
        {
            public Soldier Soldier { get; set; }
        }
    }
}