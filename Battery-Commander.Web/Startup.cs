﻿using System;
using System.IO;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using BatteryCommander.Web.Controllers;
using BatteryCommander.Web.Jobs;
using BatteryCommander.Web.Models;
using BatteryCommander.Web.Services;
using MediatR;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WebEssentials.AspNetCore.Pwa;

namespace BatteryCommander.Web
{
    public class Startup
    {
        public static String API_Version => "v1";

        public static String API_Name => $"Battery Commander {API_Version}";

        public static String Email_Address => "BatteryCommander@redleg.app";

        private static Boolean IsDevelopment;

        public Startup(IWebHostEnvironment env, IConfiguration configuration)
        {
            IsDevelopment = env.IsDevelopment();
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            String SendGridAPIKey =
                Configuration
                .GetSection("SendGrid")
                .GetValue<String>("ApiKey");

            // Add functionality to inject IOptions<T>
            services.AddOptions();

            // Add the Auth0 Settings object so it can be injected
            services.Configure<Auth0Settings>(Configuration.GetSection("Auth0"));

            var auth0Settings = new Auth0Settings { };

            Configuration.GetSection("Auth0").Bind(auth0Settings);

            services.AddTransient<ReportService>();

            // Register jobs as services for IoC
            services
                .AddTransient<SqliteBackupJob>()
                .AddTransient<EvaluationDueReminderJob>()
                .AddTransient<PERSTATReportJob>()
                .AddTransient<SensitiveItemsReport>()
                .AddTransient<EvaluationStatusChangeJob>();

            services
                .AddHttpContextAccessor()
                .AddMemoryCache()
                .AddMediatR(typeof(Startup))
                .AddDbContext<Database>();

            services
                .AddFluentEmail(defaultFromEmail: Email_Address, defaultFromName: "Battery Commander App")
                .AddRazorRenderer()
                .AddSendGridSender(SendGridAPIKey);

            // Add authentication services

            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                })
                .AddCookie(o =>
                {
                    o.LoginPath = new PathString("/Home/Login");
                    o.Cookie.SameSite = SameSiteMode.None;
                })
                .AddOpenIdConnect("Auth0", options =>
                {
                    options.GetClaimsFromUserInfoEndpoint = true;

                    // Set the authority to your Auth0 Domain
                    options.Authority = $"https://{auth0Settings.Domain}/";

                    // Configure the Auth0 Client ID and Client Secret
                    options.ClientId = auth0Settings.ClientId;
                    options.ClientSecret = auth0Settings.ClientSecret;

                    // Set response type to code
                    options.ResponseType = "code";

                    // Configure the scope
                    options.Scope.Clear();
                    options.Scope.Add("openid");

                    // Set the callback path, so Auth0 will call back to http://localhost:5000/signin-auth0
                    // Also ensure that you have added the URL as an Allowed Callback URL in your Auth0 dashboard
                    options.CallbackPath = new PathString("/signin-auth0");

                    // Configure the Claims Issuer to be Auth0
                    options.ClaimsIssuer = "Auth0";

                    options.Events = new Microsoft.AspNetCore.Authentication.OpenIdConnect.OpenIdConnectEvents
                    {
                        OnRedirectToIdentityProvider = (context) =>
                        {
                            if (context.Request.Path.StartsWithSegments("/api"))
                            {
                                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                                return Task.CompletedTask;
                            }

                            context.Response.Redirect(context.Options.SignedOutRedirectUri);

                            return Task.CompletedTask;
                        },

                        OnTicketReceived = (context) =>
                        {
                            var identity = context.Principal.Identity as ClaimsIdentity;

                            if (identity != null)
                            {
                                var name = identity.FindFirst("email");

                                if (name != null)
                                {
                                    identity.AddClaim(new Claim(ClaimTypes.Name, name.Value));
                                }
                            }

                            return Task.CompletedTask;
                        }
                    };
                });

            services.ConfigureApplicationCookie(options => options.Cookie.SameSite = SameSiteMode.None);
            services.ConfigureExternalCookie(options => options.Cookie.SameSite = SameSiteMode.None);

            services
                .AddDataProtection()
                .PersistKeysToDbContext<Database>();

            services.AddControllersWithViews(options =>
            {
                // Require auth on all pages unless explicitly anonymous

                var p =
                    new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();

                options.Filters.Add(new AuthorizeFilter(p));
            });

            // Register the Swagger generator, defining one or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(API_Version, new Microsoft.OpenApi.Models.OpenApiInfo { Title = API_Name, Version = API_Version });

                c.CustomSchemaIds(x => x.FullName);

                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "BatteryCommander.Web.xml"));
            });

            services.AddProgressiveWebApp(new PwaOptions
            {
                Strategy = ServiceWorkerStrategy.CacheFirst,
                RegisterWebmanifest = false
            });

            services.AddMediatR(typeof(Startup).Assembly);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory, Database db)
        {
            app.UseDeveloperExceptionPage();

            app.UseStatusCodePages();

            app.UseStaticFiles();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", API_Name);
            });

            app
                .UseAuthentication()
                .Use(async (context, next) =>
                {
                    // Are we already in the pipeline for our request access page? 'cuz it wouldn't make sense to redirect again

                    if (context.Request.Path.HasValue)
                    {
                        if (!context.Request.Path.Value.Contains(nameof(HomeController.RequestAccess)))
                        {
                            using (var scope = context.RequestServices.CreateScope())
                            {
                                // Get the user in the system, if they exist, by email address

                                var database = scope.ServiceProvider.GetService<Database>();

                                var user = await UserService.FindAsync(database, context.User);

                                // Check if the user has been granted access to the system

                                if (user?.CanLogin != true)
                                {
                                    // If not, redirect to request access page

                                    context.Response.Redirect($"{context.Request.PathBase}/Home/RequestAccess");
                                }
                            }
                        }
                    }

                    await next.Invoke();
                });

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });

            app.UseCors("Policy");

            app.UseJobScheduler(loggerFactory);

            Database.Init(db);
        }
    }
}