using BatteryCommander.Web.Models;
using BatteryCommander.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BatteryCommander.Web.Controllers.API
{
    public class UsersController : ApiController
    {
        private static readonly HttpClient http = new HttpClient();

        private readonly IOptions<Auth0Settings> auth0;

        public UsersController(Database db, IOptions<Auth0Settings> auth0) : base(db)
        {
            this.auth0 = auth0;
        }

        [HttpPost("~/api/auth"), AllowAnonymous]
        public async Task<IActionResult> Authenticate(String username, String password)
        {
            var request = JsonConvert.SerializeObject(new
            {
                client_id = auth0.Value.ClientId,
                client_secret = auth0.Value.ClientSecret,
                audience = auth0.Value.ApiIdentifier,
                grant_type = "password",
                username,
                password
            });

            var result = await http.PostAsync($"https://{auth0.Value.Domain}/oauth/token", new StringContent(request, Encoding.UTF8, "application/json"));

            var json = await result.Content.ReadAsStringAsync();

            var model = JsonConvert.DeserializeAnonymousType(json, new { access_token = "", token_type = "", expires_in = 0 });

            return Json(model);
        }
    }
}