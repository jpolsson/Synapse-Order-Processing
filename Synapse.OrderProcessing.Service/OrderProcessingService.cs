using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Synapse.OrderProcessing.Core.Interfaces;
using Synapse.OrderProcessing.Core.Services;


namespace Synapse.OrderProcessing.Service
{
    internal class OrderProcessingService : BackgroundService
    {
        private readonly INotificationService _notificationService;
        private readonly IOrdersService _ordersService;
        public OrderProcessingService(INotificationService notificationService, IOrdersService ordersService)
        {
            _notificationService = notificationService;
            _ordersService = ordersService;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Log.Information("Order Processing Service is starting.");
            Core.Services.OrderProcessingService service = new Core.Services.OrderProcessingService(_notificationService, _ordersService);
            while (!stoppingToken.IsCancellationRequested)
            {
                Log.Information("Calling Process Orders: {time}", DateTimeOffset.Now);
                await service.ProcessOrders();
                // Wait for 5 minutes before the next iteration
                try
                {
                    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                }
                catch (TaskCanceledException)
                {
                    // Task was canceled, exit the loop
                    break;
                }
            }

            Log.Information("Order Processing is stopping.");
        }
    }
}
