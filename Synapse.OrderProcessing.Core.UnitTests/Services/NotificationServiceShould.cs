using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using Synapse.OrderProcessing.Core.Models;
using Synapse.OrderProcessing.Core.UnitTests.Utilities;

namespace Synapse.OrderProcessing.Core.UnitTests.Services
{
    internal class NotificationServiceShould
    {
        private Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private Mock<IConfiguration> _configurationMock;
        private HttpClient _httpClient;

        private void ClearMocks()
        {
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            _configurationMock = new Mock<IConfiguration>();
        }
        [Test]
        public async Task PostToNotificationApiOnceOnSuccess()
        {
            //Arrange
            var testOrderId = Guid.NewGuid().ToString();
            var testDescription = "test order description";
            var testStatus = "test order status";
            var testDeliveryNotification = 0;


            var itemDataString = TestUtilities.BuildJTokenString(testOrderId, testDescription, testStatus, testDeliveryNotification);
            var jtoken = TestUtilities.BuildJToken(itemDataString);
            var orderItem = new EquipmentOrderItem(jtoken);
            ClearMocks();
            var notificationService = TestUtilities.BuildNotificationService(_httpMessageHandlerMock,_configurationMock,_httpClient);
            //Act
            var success = await notificationService.SendNotification(orderItem);
            //Assert
            Assert.IsTrue(success, "Expected SendNotification to return true on a successful post.");

            _httpMessageHandlerMock.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>());

        }

        [Test]
        public async Task PostToNotificationApiNTimeOnFailure()
        {
            //Note Test utilities setting retry max to 3
            //Arrange
            var testOrderId = Guid.NewGuid().ToString();
            var testDescription = "test order description";
            var testStatus = "test order status";
            var testDeliveryNotification = 0;

            var itemDataString = TestUtilities.BuildJTokenString(testOrderId, testDescription, testStatus, testDeliveryNotification);
            var jtoken = TestUtilities.BuildJToken(itemDataString);
            var orderItem = new EquipmentOrderItem(jtoken);
            ClearMocks();
            var notificationService = TestUtilities.BuildNotificationService(_httpMessageHandlerMock, _configurationMock, _httpClient, false);
            //Act
            var success = await notificationService.SendNotification(orderItem);
            //Assert
            Assert.IsFalse(success, "Expected SendNotification to return true on a successful post.");

            _httpMessageHandlerMock.Protected().Verify(
                "SendAsync",
                Times.Exactly(3),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>());

        }
    }
}
