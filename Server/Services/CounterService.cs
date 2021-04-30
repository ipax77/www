using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using www.pwa.Server.Contexts;
using Microsoft.Extensions.DependencyInjection;

namespace www.pwa.Server.Services
{
    public class CounterService : IHostedService, IDisposable
    {
        private int executionCount = 0;
        private readonly ILogger<CounterService> _logger;
        private readonly IServiceProvider _sp;
        private Timer _timer;

        public CounterService(ILogger<CounterService> logger, IServiceProvider sp)
        {
            _logger = logger;
            _sp = sp;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Counter Service running.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromHours(1));

            return Task.CompletedTask;
        }

        private async void DoWork(object state)
        {
            var count = Interlocked.Increment(ref executionCount);
            using (var scope = _sp.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<WwwContext>();
                await DbService.SetCounters(context);
            }
            _logger.LogInformation(
                "Counter Service is working. Count: {Count}", count);
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Counter Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
