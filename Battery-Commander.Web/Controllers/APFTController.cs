using BatteryCommander.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace BatteryCommander.Web.Controllers
{
    [Authorize]
    public class APFTController : Controller
    {
        private readonly Database db;

        public APFTController(Database db)
        {
            this.db = db;
        }

        public async Task<IActionResult> Index()
        {
            throw new NotImplementedException();
        }

        public async Task<IActionResult> Details(int id)
        {
            throw new NotImplementedException();
        }

        public IActionResult New()
        {
            throw new NotImplementedException();
        }

        public async Task<IActionResult> Edit(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IActionResult> Save(dynamic model)
        {
            // If EXISTS, Update

            // Else, Create New

            await db.SaveChangesAsync();

            return RedirectToAction(nameof(Details), model.Id);
        }
    }
}