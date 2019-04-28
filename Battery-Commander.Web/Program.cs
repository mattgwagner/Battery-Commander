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
            using (SentrySdk.Init())
            {
                var host = new WebHostBuilder()
                    .UseApplicationInsights()
                    .UseKestrel()
                    .UseContentRoot(Directory.GetCurrentDirectory())
                    .UseIISIntegration()
                    .UseStartup<Startup>()
                    .UseSentry()
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