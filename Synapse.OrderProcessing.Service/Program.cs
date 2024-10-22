using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Synapse.OrderProcessing.Core.Interfaces;
using Synapse.OrderProcessing.Core.Services;


namespace Synapse.OrderProcessing.Service
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            // Configure Serilog
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("System.Net.Http.HttpClient", Serilog.Events.LogEventLevel.Warning) // Filter out HttpClient logs below Warning
                .WriteTo.Console()
                .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            try
            {
                Log.Information("Starting the application");

                // Set up the Host with Dependency Injection and Serilog
                var host = Host.CreateDefaultBuilder(args)
                    .UseSerilog() // Use Serilog for logging
                    .ConfigureAppConfiguration((hostingContext, config) =>
                    {
                        var env = hostingContext.HostingEnvironment;

                        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                            .AddEnvironmentVariables();
                    })
                   .ConfigureServices((context, services) =>
                    {
                        services.AddHttpClient<NotificationService>();
                        services.AddSingleton<INotificationService, NotificationService>();
                        services.AddSingleton<IOrdersService, OrdersService>();
                        services.AddHostedService<OrderProcessingService>();
                    })
                    .Build();

                // Run the application
                await host.RunAsync();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
