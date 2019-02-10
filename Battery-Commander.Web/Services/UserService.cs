﻿using BatteryCommander.Web.Models;
using FluentEmail.Core;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BatteryCommander.Web.Services
{
    public class UserService
    {
        internal static TimeSpan CacheDuration => TimeSpan.FromDays(1);

        /// <summary>
        /// Find the Soldier associated with the given user.
        /// Will throw an exception if more than Soldier is found.
        /// Will return null if none was found.
        /// </summary>
        public static async Task<Soldier> FindAsync(Database db, ClaimsPrincipal user)
        {
            var email = Get_Email(user);

            if (String.IsNullOrWhiteSpace(email)) return null;

            var soldiers = await SoldierService.Filter(db, new SoldierService.Query { Email = email });

            if (soldiers.Count() > 1) throw new Exception($"Found multiple matching soldiers with the same email: {email}");

            var soldier = soldiers.SingleOrDefault();

            if(soldier != null)
            {
                db.Entry(soldier).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
            }

            return soldier;
        }

        public static void RequestAccess(IFluentEmailFactory emailSvc, RequestAccessModel model)
        {
            emailSvc
                .Create()
                .To("Access@RedLeg.app")
                .ReplyTo(model.Email)
                .Subject($"Access Request | {model.Name}")
                .UsingTemplateFromFile($"{Directory.GetCurrentDirectory()}/Views/Home/RequestAccessEmail.cshtml", model)
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