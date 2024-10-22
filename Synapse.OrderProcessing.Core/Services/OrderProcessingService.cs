using Serilog;
using  Synapse.OrderProcessing.Core.Interfaces;
using Synapse.OrderProcessing.Core.Models;

namespace  Synapse.OrderProcessing.Core.Services
{
    public class OrderProcessingService : IOrdersProcessingService
    {
        private readonly INotificationService _notificationService;
        private readonly IOrdersService _ordersService;
        public OrderProcessingService(INotificationService notificationService, IOrdersService ordersService) {
            _notificationService = notificationService;
            _ordersService = ordersService;
        }
        public async Task<bool> ProcessOrders()
        {
            try
            {
                var orders = await _ordersService.GetOrders();
                var deliveredOrders = orders
                    .Where(o => o.Items.Any(item => item.Status.Equals("Delivered", StringComparison.OrdinalIgnoreCase)))
                    .ToList();
                Log.Information($"{deliveredOrders.Count} orders found with delivered Items");
                foreach (var order in deliveredOrders) {
                    Log.Information($"Processing Order {order.Id}");
                    await ProcessDeliveredItems(order);
                }
                return true;

            }
            catch (Exception ex)
            {
                Log.Error($"unexpected error in ProcessOrders: {ex.Message}", ex);
                return false;
            }
        }

        private async Task ProcessDeliveredItems(EquipmentOrder order)
        {
            // Filter delivered items
            var deliveredItems = order.Items
            .Where(item => item.Status.Equals("Delivered", StringComparison.OrdinalIgnoreCase))
            .ToList();
            foreach (var item in deliveredItems)
            {
                Log.Information($"Sending Notification for Order: {item.OrderId} Item: {item.Description}");
                var success = await _notificationService.SendNotification(item);
                if (success)
                {
                    item.DeliveryNotification += 1;
                    Log.Information($"Notification Sent for order: {item.OrderId} Item: {item.Description}. Notification count: {item.DeliveryNotification}");
                }
                else
                {
                    Log.Error($"Unexpected error sending notification for order: {item.OrderId} item: {item.Description} Notification count: {item.DeliveryNotification}");
                }
            }
            //sync item deliveryNotification updates to JObject
            order.SyncOrderJObject();
            var updateSuccess = await _ordersService.UpdateOrder(order.Order);
            if (updateSuccess)
            {
                Log.Information($"Order {order.Id} updated successfully");
            }
            else
            {
                Log.Error($"Update for order {order.Id} failed.");
            }
        }
    }
}
