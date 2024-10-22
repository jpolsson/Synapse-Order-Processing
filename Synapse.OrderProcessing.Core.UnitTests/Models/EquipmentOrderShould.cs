using Newtonsoft.Json.Linq;
using Synapse.OrderProcessing.Core.Models;
using Synapse.OrderProcessing.Core.UnitTests.Utilities;


namespace Synapse.OrderProcessing.Core.UnitTests.Models
{
    internal class EquipmentOrderShould
    {
        [Test]
        public void PopulatePropsFromJobjectOnInit()
        {
            //Arrange
            string projectDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string testDataPath = Path.Combine(projectDirectory, "..", "..", "..", "Utilities", "TestOrderData.json");
            var jObjectArray = TestUtilities.LoadTestData(testDataPath); 
            //Act
            var testEquipmentOrder = new EquipmentOrder(jObjectArray[0]);
            //Assert
            var numItems = testEquipmentOrder.Items.Count;
            Assert.NotNull(testEquipmentOrder);
            Assert.That(testEquipmentOrder.Order, Is.EqualTo(jObjectArray[0]));
            Assert.That(testEquipmentOrder.Id, Is.EqualTo(jObjectArray[0]?["Id"]?.ToString()));
            Assert.That(numItems, Is.EqualTo(jObjectArray[0]?["Items"]?.Count()));
            Assert.That(testEquipmentOrder.Items[0].OrderId, Is.EqualTo(jObjectArray[0]?["Items"]?[0]?["OrderId"]?.ToString()));
            Assert.That(testEquipmentOrder.Items[numItems -1].Status, Is.EqualTo(jObjectArray[0]?["Items"]?[numItems - 1]?["Status"]?.ToString()));

        }

        [Test]
        public void ThrowArgumentException_WhenOrderIsNull()
        {
            //Arrange
            JObject? order = null;
            //Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => new EquipmentOrder(order));
            Assert.That(exception.Message, Is.EqualTo("order (Parameter 'Invalid order: Order cannot be null.')"));


        }
    }
}
