using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DynuPdate
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var host = new HostBuilder()
                .ConfigureAppConfiguration(config =>
                {
                    config.AddEnvironmentVariables();
                })
                .ConfigureLogging(factory =>
                {
                    factory.AddConsole();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddOptions();

                    services.Configure<Config>(options =>
                    {
                        var hostname = Environment.GetEnvironmentVariable("DYNU_HOSTNAME");
                        var username = Environment.GetEnvironmentVariable("DYNU_USERNAME");
                        var password = Environment.GetEnvironmentVariable("DYNU_PASSWORD");
                        var interval = Environment.GetEnvironmentVariable("DYNU_INTERVAL");

                        if (string.IsNullOrWhiteSpace(hostname))
                            throw new Exception("Invalid Enviromental Variable: DYNU_HOSTNAME");
                        if (string.IsNullOrWhiteSpace(username))
                            throw new Exception("Invalid Enviromental Variable: DYNU_USERNAME");
                        if (string.IsNullOrWhiteSpace(password))
                            throw new Exception("Invalid Enviromental Variable: DYNU_PASSWORD");
                        if (string.IsNullOrWhiteSpace(interval))
                            throw new Exception("Invalid Enviromental Variable: DYNU_INTERVAL");

                        options.Hostname = hostname;
                        options.Interval = int.Parse(interval);
                        options.Username = username;
                        options.Password = password;

                    });

                    services.AddHostedService<TimedHostedService>();

                })
                .Build();

            using (host)
            {
                // start the MSMQ host
                await host.StartAsync();

                // read and dispatch messages to the MSMQ queue
                //StartReadLoop(host);

                // wait for the MSMQ host to shutdown
                await host.WaitForShutdownAsync();
                await host.StopAsync();
            }
        }

        private static void StartReadLoop(IHost host)
        {
            //var connection = host.Services.GetRequiredService<IMsmqConnection>();
            var applicationLifetime = host.Services.GetRequiredService<IApplicationLifetime>();

            //// run the read loop in a background thread so that it can be stopped with CTRL+C
            System.Threading.Tasks.Task.Run(() => ReadLoop(applicationLifetime.ApplicationStopping));
        }

        private static void ReadLoop(CancellationToken cancellationToken)
        {
            Console.WriteLine("Enter your text message and press ENTER...");

            while (!cancellationToken.IsCancellationRequested)
            {

            }
        }

    }
}