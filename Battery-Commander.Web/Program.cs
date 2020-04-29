namespace BatteryCommander.Web
{
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;
    using Serilog;
    using Serilog.Core;

    public class Program
    {
        public static LoggingLevelSwitch LogLevel { get; } = new LoggingLevelSwitch(Serilog.Events.LogEventLevel.Information);

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .UseSentry(dsn: "https://78e464f7456f49a98e500e78b0bb4b13@o255975.ingest.sentry.io/1447369")
                        .UseStartup<Startup>();
                })
                .ConfigureLogging((context, builder) =>
                {
                    Log.Logger =
                           new LoggerConfiguration()
                           .Enrich.FromLogContext()
                           .Enrich.WithProperty("Application", context.HostingEnvironment.ApplicationName)
                           .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
                           .Enrich.WithProperty("Version", $"{typeof(Startup).Assembly.GetName().Version}")
                           .WriteTo.Seq(serverUrl: "http://redlegdev-logs.eastus.azurecontainer.io", apiKey: context.Configuration.GetValue<string>("Seq:ApiKey"), compact: true, controlLevelSwitch: LogLevel)
                           .MinimumLevel.ControlledBy(LogLevel)
                           .CreateLogger();

                    builder.AddSerilog();
                });
    }
}
