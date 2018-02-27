using BatteryCommander.Web.Models;
using BatteryCommander.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Net;
using System.Threading.Tasks;

namespace BatteryCommander.Web.Controllers.API
{
    public class UsersController : ApiController
    {
        private readonly IMemoryCache cache;

        private async Task<Soldier> CurrentUser() => await UserService.FindAsync(db, User, cache);

        public UsersController(IMemoryCache cache, Database db) : base(db)
        {
            this.cache = cache;
        }

        [HttpGet("me")]
        public async Task<dynamic> Current()
        {
            // GET: api/users/me

            var user = await CurrentUser();

            if (user == null) return StatusCode((int)HttpStatusCode.BadRequest);

            return new
            {
                user.Id,
                user.FirstName,
                user.LastName,
                user.CivilianEmail
            };
        }
    }
}