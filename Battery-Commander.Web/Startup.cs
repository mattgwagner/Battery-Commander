using BatteryCommander.Web.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BatteryCommander.Web
{
    public class Startup
    {
        private static Boolean IsDevelopment;

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
            IsDevelopment = env.IsDevelopment();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add authentication services
            services.AddAuthentication(options => options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme);

            // Add framework services.
            services.AddMvc(options =>
            {
                if (!IsDevelopment)
                {
                    options.Filters.Add(new RequireHttpsAttribute { });
                }
            });

            // Add functionality to inject IOptions<T>
            services.AddOptions();

            // Add the Auth0 Settings object so it can be injected
            services.Configure<Auth0Settings>(Configuration.GetSection("Auth0"));

            // Add the Twilio settings so it can be injected
            services.Configure<TwilioSettings>(Configuration.GetSection("Twilio"));

            services.AddDbContext<Database>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IOptions<Auth0Settings> auth0Settings, Database db)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
            }

            app.UseDeveloperExceptionPage();

            app.UseStaticFiles();

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AutomaticChallenge = true,
                LoginPath = new PathString("/Home/Login")
            });

            var options = new OpenIdConnectOptions("Auth0")
            {
                // Set the authority to your Auth0 Domain
                Authority = $"https://{auth0Settings.Value.Domain}",

                // Configure the Auth0 Client ID and Client Secret
                ClientId = auth0Settings.Value.ClientId,
                ClientSecret = auth0Settings.Value.ClientSecret,

                // Do not automatically authenticate and challenge
                AutomaticAuthenticate = false,
                AutomaticChallenge = false,

                // Set response type to code
                ResponseType = "code",

                // Set the callback path, so Auth0 will call back to http://localhost:5000/signin-auth0
                // Also ensure that you have added the URL as an Allowed Callback URL in your Auth0 dashboard
                CallbackPath = new PathString("/signin-auth0"),

                // Configure the Claims Issuer to be Auth0
                ClaimsIssuer = "Auth0",

                // Saves tokens to the AuthenticationProperties
                SaveTokens = true,

                Events = new OpenIdConnectEvents
                {
                    OnTicketReceived = (context) =>
                    {
                        var identity = context.Principal.Identity as ClaimsIdentity;

                        if (identity != null)
                        {
                            var name = identity.FindFirst("name");

                            if (name != null)
                            {
                                identity.AddClaim(new Claim(ClaimTypes.Name, name.Value));
                            }
                        }

                        return Task.CompletedTask;
                    },

                    OnRedirectToIdentityProviderForSignOut = (context) =>
                    {
                        var logoutUri = $"https://{auth0Settings.Value.Domain}/v2/logout?client_id={auth0Settings.Value.ClientId}";

                        var postLogoutUri = context.Properties.RedirectUri;
                        if (!string.IsNullOrEmpty(postLogoutUri))
                        {
                            if (postLogoutUri.StartsWith("/"))
                            {
                                // transform to absolute
                                var request = context.Request;
                                postLogoutUri = request.Scheme + "://" + request.Host + request.PathBase + postLogoutUri;
                            }
                            logoutUri += $"&returnTo={ Uri.EscapeDataString(postLogoutUri) }";
                        }

                        context.Response.Redirect(logoutUri);
                        context.HandleResponse();

                        return Task.CompletedTask;
                    }
                }
            };

            options.Scope.Clear();
            options.Scope.Add("openid");
            options.Scope.Add("name");
            options.Scope.Add("email");

            app.UseOpenIdConnectAuthentication(options);

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Units}/{action=Index}/{id?}");
            });

            Database.Init(db);
        }
    }
}
