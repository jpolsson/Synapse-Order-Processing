using  Synapse.OrderProcessing.Core.Models;


namespace  Synapse.OrderProcessing.Core.Interfaces
{
    public interface INotificationService
    {
        Task<bool> SendNotification(EquipmentOrderItem item);
    }
}
