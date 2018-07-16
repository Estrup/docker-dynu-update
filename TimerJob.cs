using FluentScheduler;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DynuPdate
{
    #region snippet1
    public class TimedHostedService : IHostedService, IDisposable
    {
        private readonly ILogger _logger;

        private Registry _registry;

        public TimedHostedService(ILogger<TimedHostedService> logger, IOptions<Config> config)
        {
            var options = config.Value;

            _logger = logger;
            _registry = new Registry();
            _registry.Schedule(() =>
            {
                try
                {
                    var iptask = IpHelp.GetIp();
                    iptask.Wait();
                    var ip = iptask.Result;
                    Console.WriteLine(iptask.Result);

                    var updatetask = IpHelp.UpdateIp(options.Hostname, ip, options.Username, options.Password);
                    updatetask.Wait();
                }
                catch(Exception ex){
                    Console.WriteLine($"Error: { ex.Message }");
                    _logger.LogError($"Error: { ex.Message }", ex);
                }

            }).ToRunEvery(options.Interval).Seconds();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is starting.");

            JobManager.Initialize(_registry);

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            _logger.LogInformation("Timed Background Service is working.");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is stopping.");

            //JobManager.RemoveJob("")

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            
        }
    }
    #endregion
}
