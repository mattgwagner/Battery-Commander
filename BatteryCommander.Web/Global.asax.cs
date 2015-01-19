using BatteryCommander.Common;
using BatteryCommander.Common.Models;
using BatteryCommander.Common.Services.Notifications;
using BatteryCommander.Common.Services.Users;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using SimpleInjector;
using SimpleInjector.Integration.Web.Mvc;
using System.Configuration;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace BatteryCommander.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            RegisterIoc();

            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);

            RegisterRoutes(RouteTable.Routes);

            RegisterBundles(BundleTable.Bundles);
        }

        public static void RegisterIoc()
        {
            var container = new Container();

            container.Register<DataContext>();

            container.Register<IUserStore<AppUser, int>, UserService>();

            container.Register<UserManager<AppUser, int>, AppUserManager>();

            container.Register<SMSService.Config>(() =>
                {
                    return new SMSService.Config
                    {
                        TwilioAccountSid = ConfigurationManager.AppSettings["Twilio.AccountSid"],
                        TwilioAuthToken = ConfigurationManager.AppSettings["Twilio.AuthToken"],
                        TwilioFromNumber = ConfigurationManager.AppSettings["Twilio.FromNumber"]
                    };
                });

            container.Register<IIdentityMessageService, SMSService>();

            container.Register<EmailService.Config>(() =>
                {
                    return new EmailService.Config
                    {
                        Host = ConfigurationManager.AppSettings["MAILGUN_SMTP_SERVER"],
                        Port = int.Parse(ConfigurationManager.AppSettings["MAILGUN_SMTP_PORT"] ?? "25"),
                        Username = ConfigurationManager.AppSettings["MAILGUN_SMTP_LOGIN"],
                        Password = ConfigurationManager.AppSettings["MAILGUN_SMTP_PASSWORD"]
                    };
                });

            container.Register<EmailService>();

            container.RegisterMvcControllers(Assembly.GetExecutingAssembly());

            container.RegisterPerWebRequest<IAuthenticationManager>(() => HttpContext.Current.GetOwinContext().Authentication);

            container.RegisterPerWebRequest<SignInManager<AppUser, int>>();

            DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(container));
        }

        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new System.Web.Mvc.HandleErrorAttribute());
            filters.Add(new System.Web.Mvc.AuthorizeAttribute());

            // We're going to require HTTPS for all pages outside of running locally
#if !DEBUG
            filters.Add(new System.Web.Mvc.RequireHttpsAttribute());
#endif
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapMvcAttributeRoutes();

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }

        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));
        }
    }
}