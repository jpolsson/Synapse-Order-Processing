using Synapse.OrderProcessing.Core.Models;
using Synapse.OrderProcessing.Core.UnitTests.Utilities;


namespace Synapse.OrderProcessing.Core.UnitTests.Models
{
    internal class EquipmentOrderItemShould
    {
       
        [Test]
        public void PopulatePropsFromJobjectOnInit()
        {
            //Arrange
            var testOrderId = Guid.NewGuid().ToString();
            var testDescription = "test order description";
            var testStatus = "test order status";
            var testDeliveryNotification = 0;

            var itemDataString = TestUtilities.BuildJTokenString(testOrderId, testDescription, testStatus,testDeliveryNotification); 
            var jtoken = TestUtilities.BuildJToken(itemDataString);
            //Act
            var testEquipmentOrderItem = new EquipmentOrderItem(jtoken);
            //Assert
            Assert.NotNull(testEquipmentOrderItem);
            Assert.IsTrue(testEquipmentOrderItem.OrderId.Equals(testOrderId));
            Assert.IsTrue(testEquipmentOrderItem.Status.Equals(testStatus));
            Assert.IsTrue(testEquipmentOrderItem.Description.Equals(testDescription));
            Assert.IsTrue(testEquipmentOrderItem.DeliveryNotification.Equals(testDeliveryNotification));

        }

        [Test]
        public void PopulatePropsWithEmptyStringWhenJobjectPropsAreNull()
        {
            //Arrange
            var jtoken = TestUtilities.BuildJToken();
            //Act
            var testEquipmentOrderItem = new EquipmentOrderItem(jtoken);
            //Assert
            Assert.NotNull(testEquipmentOrderItem);
            Assert.IsTrue(testEquipmentOrderItem.OrderId.Equals(string.Empty));
            Assert.IsTrue(testEquipmentOrderItem.Status.Equals(string.Empty));
            Assert.IsTrue(testEquipmentOrderItem.Description.Equals(string.Empty));
            Assert.IsTrue(testEquipmentOrderItem.DeliveryNotification.Equals(0));

        }
    }
}
