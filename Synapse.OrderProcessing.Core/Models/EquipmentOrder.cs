using Newtonsoft.Json.Linq;

namespace  Synapse.OrderProcessing.Core.Models
{
    public class EquipmentOrder
    {
        public JObject Order { get; set; } = null!;
        public List<EquipmentOrderItem> Items { get; set; } = null!;
        public string Id { get; set; } = null!;

        public EquipmentOrder()
        {
                
        }
        public EquipmentOrder(JObject order)
        {
            if (order is null || order["Id"] is null || order["Id"]?.ToString() == string.Empty)
            {
                throw new ArgumentNullException("Invalid order: Order cannot be null.",nameof(order));
            }
            Order = order;
            Items = order["Items"]?
                    .ToObject<JArray>()?
                    .Select(item => new EquipmentOrderItem(item))
                    .ToList() ?? new List<EquipmentOrderItem>();
            Id = order["Id"]?.ToString() ?? string.Empty;

        }

        public void SyncOrderJObject()
        {
            // Convert the updated Items list back to a JArray
            var updatedItemsArray = new JArray(
                Items.Select(item => new JObject
                {
                    ["OrderId"] = item.OrderId,
                    ["Description"] = item.Description,
                    ["Status"] = item.Status,
                    ["DeliveryNotification"] = item.DeliveryNotification
                })
            );
            // Update the original JObject's "Items" property
            Order["Items"] = updatedItemsArray;
        }
    }
}
