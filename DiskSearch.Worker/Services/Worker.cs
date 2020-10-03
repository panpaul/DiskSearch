using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DiskSearch.Worker.Services
{
    public class Worker : BackgroundService
    {
        public static readonly Backend Backend = new Backend(
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "DiskSearch"
            ));

        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.CompletedTask;
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("DiskSearch.Worker Stopping...");
            Backend.UnWatch();
            Backend.Close();

            await base.StopAsync(stoppingToken);
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("DiskSearch.Worker Starting...");
            Backend.DefaultSetup();

            return base.StartAsync(cancellationToken);
        }
    }
}