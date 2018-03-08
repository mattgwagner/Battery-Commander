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
using Newtonsoft.Json.Converters;
using Serilog;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.IO;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BatteryCommander.Web
{
    public class Startup
    {
        public static String API_Version => "v1";

        public static String API_Name => $"Battery Commander {API_Version}";

        public static String APP_INSIGHTS_KEY = "66d7081f-e4a1-421f-b57a-38656917ee3d";

        private static Boolean IsDevelopment;

        public Startup(IHostingEnvironment env)
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.RollingFile(pathFormat: @"logs\{Date}.log")
                .WriteTo.ApplicationInsightsTraces(APP_INSIGHTS_KEY)
                .MinimumLevel.Information()
                .CreateLogger();

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
            services.AddMemoryCache();

            // Add functionality to inject IOptions<T>
            services.AddOptions();

            // Add the Auth0 Settings object so it can be injected
            services.Configure<Auth0Settings>(Configuration.GetSection("Auth0"));

            var auth0Settings = new Auth0Settings { };

            Configuration.GetSection("Auth0").Bind(auth0Settings);

            services.AddCors(options =>
            {
                options.AddPolicy("Policy", builder =>
                    builder
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowAnyOrigin()
                        .AllowCredentials()
                        .Build()
                    );
            });

            // Add authentication services

            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(o =>
                {
                    o.Authority = $"https://{auth0Settings.Domain}/";
                    o.Audience = auth0Settings.ApiIdentifier;
                    o.RequireHttpsMetadata = !IsDevelopment;

                    //o.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    //{
                    //    ValidateIssuer = true,
                    //    ValidateAudience = true,
                    //    ValidateLifetime = true,

                    //    ValidateIssuerSigningKey = true,
                    //    ValidIssuer = "",
                    //    ValidAudience = auth0Settings.ApiIdentifier
                    //};
                })
                .AddCookie(o => o.LoginPath = new PathString("/Home/Login"))
                .AddOpenIdConnect("Auth0", options =>
                {
                    options.GetClaimsFromUserInfoEndpoint = true;

                    // Set the authority to your Auth0 Domain
                    options.Authority = $"https://{auth0Settings.Domain}";

                    // Configure the Auth0 Client ID and Client Secret
                    options.ClientId = auth0Settings.ClientId;
                    options.ClientSecret = auth0Settings.ClientSecret;

                    // Do not automatically authenticate and challenge
                    //options.AutomaticAuthenticate = false;
                    //options.AutomaticChallenge = false;

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

                    options.Events = new OpenIdConnectEvents
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
                        },

                        OnRedirectToIdentityProviderForSignOut = (context) =>
                        {
                            var logoutUri = $"https://{auth0Settings.Domain}/v2/logout?client_id={auth0Settings.ClientId}";

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
                    };
                });

            // Add framework services.
            services.AddMvc(options =>
            {
                if (!IsDevelopment)
                {
                    options.Filters.Add(new RequireHttpsAttribute { });
                }
            })
            .AddJsonOptions(options =>
            {
                options.SerializerSettings.Converters.Add(new StringEnumConverter { });
            });

            // Add functionality to inject IOptions<T>
            services.AddOptions();

            services.AddDbContext<Database>();

            // Register the Swagger generator, defining one or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.DescribeAllEnumsAsStrings();

                c.SwaggerDoc(API_Version, new Info { Title = API_Name, Version = API_Version });

                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "BatteryCommander.Web.xml"));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, Database db)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddSerilog();
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
            }

            app.UseDeveloperExceptionPage();

            app.UseStaticFiles();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", API_Name);
            });

            app.UseAuthentication();

            app.UseCors("Policy");

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            Database.Init(db);
        }
    }
}