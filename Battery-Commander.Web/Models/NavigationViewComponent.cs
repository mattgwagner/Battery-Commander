using BatteryCommander.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BatteryCommander.Web.Models
{
    public class NavigationViewComponent : ViewComponent
    {
        private readonly IMemoryCache cache;
        private readonly Database db;

        public NavigationViewComponent(IMemoryCache cache, Database db)
        {
            this.cache = cache;
            this.db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync(ClaimsPrincipal user)
        {
            return View(new ViewModel { Soldier = await UserService.FindAsync(db, user, cache) });
        }

        public class ViewModel
        {
            public Soldier Soldier { get; set; }
        }
    }
}