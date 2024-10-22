namespace  Synapse.OrderProcessing.Core.Interfaces
{
    public interface IOrdersProcessingService
    {
        Task<bool> ProcessOrders();
    }
}
