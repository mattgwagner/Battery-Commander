using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Serilog;

namespace BatteryCommander.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host =
                WebHost
                .CreateDefaultBuilder(args)
                .UseSerilog((h, context) =>
                {
                    context
                    .Enrich.FromLogContext()
                    .WriteTo.Sentry();
                })
                .UseSentry(dsn: "https://78e464f7456f49a98e500e78b0bb4b13@sentry.io/1447369")
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}