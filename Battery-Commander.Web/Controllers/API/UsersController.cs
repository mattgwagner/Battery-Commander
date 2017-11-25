﻿using BatteryCommander.Web.Models;
using BatteryCommander.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;

namespace BatteryCommander.Web.Controllers.API
{
    [Route("api/[controller]"), Authorize]
    public class UsersController : Controller
    {
        private readonly Database db;

        private async Task<Soldier> CurrentUser() => await UserService.FindAsync(db, User);

        public UsersController(Database db)
        {
            this.db = db;
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