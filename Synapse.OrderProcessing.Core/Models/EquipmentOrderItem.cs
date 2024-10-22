using Newtonsoft.Json.Linq;


namespace  Synapse.OrderProcessing.Core.Models
{
    public class EquipmentOrderItem
    {
        public string OrderId { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Status { get; set; } = null!;
        public int DeliveryNotification { get; set; }

        public EquipmentOrderItem(JToken item)
        {
            OrderId = item["OrderId"]?.ToString() ?? string.Empty;
            Description = item["Description"]?.ToString() ?? string.Empty;
            Status = item["Status"]?.ToString() ?? string.Empty;
            DeliveryNotification = item.Value<int?>("DeliveryNotification") ?? 0;
        }
    }
}
