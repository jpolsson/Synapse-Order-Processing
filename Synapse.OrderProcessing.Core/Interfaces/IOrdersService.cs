using Newtonsoft.Json.Linq;
using  Synapse.OrderProcessing.Core.Models;

namespace  Synapse.OrderProcessing.Core.Interfaces
{
    public interface IOrdersService
    {
        Task<List<EquipmentOrder>> GetOrders();
        Task<bool> UpdateOrder(JObject order);
    }
}
