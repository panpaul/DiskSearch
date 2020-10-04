using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sentry.Extensibility;
using Sentry.Protocol;

namespace DiskSearch.Worker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .UseSystemd()
                .UseConsoleLifetime()
                .ConfigureServices(services => { services.AddHostedService<Services.Worker>(); })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseSentry(o =>
                    {
                        o.Dsn = "https://examplePublicKey@o0.ingest.sentry.io/0";

                        o.AttachStacktrace = true;
                        o.Debug = false;
                        o.DiagnosticsLevel = SentryLevel.Warning;
                        o.MaxBreadcrumbs = 50;
                        o.MaxRequestBodySize = RequestSize.Medium;
                        o.MinimumEventLevel = LogLevel.Warning;
                        o.SendDefaultPii = true;
                        o.ConfigureScope(scope =>
                        {
                            scope.User = new User {Id = MachineCode.MachineCode.GetMachineCode()};
                        });
                    });
                    webBuilder.UseStartup<Startup>();
                });
        }
    }
}