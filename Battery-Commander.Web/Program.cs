using Microsoft.AspNetCore.Hosting;
using Sentry;
using Serilog;
using System.IO;

namespace BatteryCommander.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using (SentrySdk.Init("https://78e464f7456f49a98e500e78b0bb4b13@sentry.io/1447369"))
            {
                var host = new WebHostBuilder()
                    .UseApplicationInsights()
                    .UseKestrel()
                    .UseContentRoot(Directory.GetCurrentDirectory())
                    .UseIISIntegration()
                    .UseStartup<Startup>()
                    .UseSerilog((h, context) =>
                    {
                        context
                        .Enrich.FromLogContext()
                        .WriteTo.Sentry();
                    })
                    .Build();

                host.Run();
            }
        }
    }
}
