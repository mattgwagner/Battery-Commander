using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BatteryCommander.Web.Models;
using FluentEmail.Core;
using Microsoft.IdentityModel.Tokens;

namespace BatteryCommander.Web.Services
{
    public class UserService
    {
        public static async Task RequestAccess(IFluentEmailFactory emailSvc, RequestAccessModel model)
        {
            await emailSvc
                .Create()
                .To("Access@RedLeg.app")
                .ReplyTo(model.Email)
                .Subject($"Access Request | {model.Name}")
                .UsingTemplateFromFile($"{Directory.GetCurrentDirectory()}/Views/Home/RequestAccessEmail.html", model)
                .SendWithErrorCheck();
        }

        public static Boolean Try_Validate_Token(String apiKey, out ClaimsPrincipal user)
        {
            SecurityToken token;

            user = Handler.ValidateToken(apiKey, new TokenValidationParameters
            {
                IssuerSigningKey = Key,
                ValidateIssuer = false,
                ValidateAudience = false
            },
            out token);

            return true;
        }

        public static String Generate_Token(ClaimsPrincipal user)
        {
            var token = new JwtSecurityToken(
                claims: new[] { new Claim("name", Get_Email(user)) },
                expires: DateTime.Today.Add(Expiry),
                signingCredentials: Credential);

            return Handler.WriteToken(token);
        }

        internal static readonly TimeSpan Expiry = TimeSpan.FromDays(1000);

        internal static readonly JwtSecurityTokenHandler Handler = new JwtSecurityTokenHandler();

        internal static readonly String InternalKey = "change-me-to-something-secure";

        internal static readonly SymmetricSecurityKey Key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(InternalKey));

        internal static readonly SigningCredentials Credential = new SigningCredentials(Key, SecurityAlgorithms.HmacSha256);

        internal static String Get_Email(ClaimsPrincipal user) => user?.Identity?.Name;
    }
}