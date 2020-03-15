using Rundeck.Tools.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Rundeck.Api.Exceptions;

namespace Rundeck.Tools
{
    public static class Program
    {
        public static CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public static async Task<int> Main(string[] args)
        {
            try
            {
                // Create the service collection passing in the configuration
                var serviceCollection = new ServiceCollection();
                var configurationRoot = BuildConfig(args);
                ConfigureServices(serviceCollection, configurationRoot);
                using var serviceProvider = serviceCollection.BuildServiceProvider();

                var application = serviceProvider.GetService<Application>();

                // Establish an event handler to process key press events.
                Console.CancelKeyPress += CancelEventHandler;

                await application
                    .RunAsync(_cancellationTokenSource.Token)
                    .ConfigureAwait(false);

                return (int)ExitCode.Ok;
            }
            catch (Rundeck.Tools.Config.ConfigurationException exception)
            {
                Console.WriteLine(exception.Message);
                return (int)ExitCode.ConfigurationIncorrect;
            }
            catch (RundeckException exception)
            {
                Console.WriteLine(exception.Message);
                return (int)ExitCode.ConfigurationIncorrect;
            }
            catch (OperationCanceledException)
            {
                // Cleanly handled.
                Console.WriteLine("Quitting.");
                return (int)ExitCode.Cancelled;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return (int)ExitCode.UnhandledException;
            }
        }

        private static void CancelEventHandler(object sender, ConsoleCancelEventArgs args)
        {
            Console.WriteLine($"Key pressed: {args.SpecialKey}");

            // Set the Cancel property to true to prevent the process from terminating.
            args.Cancel = true;

            _cancellationTokenSource?.Cancel();
        }

        private static IConfigurationRoot BuildConfig(string[] args)
        {
            // Convert appsettingsFilename to absolute path for the ConfigurationBuilder to be able to find it
            var appsettingsFilename = Path.GetFullPath("appsettings.json");

            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(appsettingsFilename, false, false)
                .AddCommandLine(args)
                .Build();
        }

        private static void ConfigureServices(IServiceCollection services, IConfiguration configuration) =>
            // Add logging
            services
            .AddLogging(b =>
                {
                    b.AddConsole();
                    b.AddDebug();
                })
            .AddOptions()
            .Configure<Configuration>(c => configuration.GetSection("Configuration").Bind(c))
            .AddTransient<Application>();
    }
}
