using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BatteryCommander.Web.Behaviors;
using BatteryCommander.Web.Jobs;
using BatteryCommander.Web.Models;
using BatteryCommander.Web.Queries;
using BatteryCommander.Web.Services;
using MediatR;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using WebEssentials.AspNetCore.Pwa;

namespace BatteryCommander.Web
{
    public class Startup
    {
        public static String API_Version => "v1";

        public static String API_Name => $"Battery Commander {API_Version}";

        public static String Email_Address => "BatteryCommander@redleg.app";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

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
                .AddTransient<SensitiveItemsReport>();

            services
                .AddHttpContextAccessor()
                .AddMemoryCache()
                .AddMediatR(typeof(Startup))
                .AddTransient(typeof(IPipelineBehavior<,>), typeof(TracingBehavior<,>))
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
                    options.Scope.Add("email");

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
                                var name = identity.FindFirst(ClaimTypes.Email);

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

                options.Conventions.Add(new ApiExplorerIgnores { });
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
                Strategy = ServiceWorkerStrategy.NetworkFirst,
                RegisterWebmanifest = false
            });
            services.AddApplicationInsightsTelemetry(Configuration["APPINSIGHTS_CONNECTIONSTRING"]);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory, Database db)
        {
            app.UseDeveloperExceptionPage();

            app.UseStatusCodePages();

            app.UseStaticFiles();

            app.UseHttpsRedirection();

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
                    using (LogContext.PushProperty("Username", context.User?.FindFirstValue(ClaimTypes.Name)))
                    {
                        await next();
                    }
                })
                .Use(async (context, next) =>
                {
                    // Are we already in the pipeline for our request access page? 'cuz it wouldn't make sense to redirect again

                    if (context.Request.Path.HasValue)
                    {
                        if (!new[] { "/RequestAccess", "/SUTA", "/Calendar" }.Any(route => context.Request.Path.StartsWithSegments(route, StringComparison.CurrentCultureIgnoreCase)))
                        {
                            using (var scope = context.RequestServices.CreateScope())
                            {
                                // Get the user in the system, if they exist, by email address

                                var user =
                                    await scope
                                    .ServiceProvider
                                    .GetService<IMediator>()
                                    .Send(new GetCurrentUser { });

                                // Check if the user has been granted access to the system

                                if (user?.CanLogin != true)
                                {
                                    // If not, redirect to request access page

                                    context.Response.Redirect($"{context.Request.PathBase}/RequestAccess");
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

    internal class ApiExplorerIgnores : IActionModelConvention
    {
        public void Apply(ActionModel action)
        {
            if (action.Controller.ControllerName.Equals("Pwa"))
                action.ApiExplorer.IsVisible = false;
        }
    }
}