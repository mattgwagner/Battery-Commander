[assembly: Microsoft.Owin.OwinStartup(typeof(BatteryCommander.Web.Startup))]

namespace BatteryCommander.Web
{
    using Microsoft.AspNet.Identity;
    using Microsoft.Owin;
    using Microsoft.Owin.Security.Cookies;
    using Owin;
    using System;

    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Auth/Login"),
                LogoutPath = new PathString("/Auth/Logout"),
                ExpireTimeSpan = TimeSpan.FromHours(1),
                SlidingExpiration = true,
                CookieHttpOnly = true,
                CookieSecure = CookieSecureOption.SameAsRequest
                // CookieDomain = "",
                // CookieName = ""
            });

            app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));

            app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);
        }
    }
}