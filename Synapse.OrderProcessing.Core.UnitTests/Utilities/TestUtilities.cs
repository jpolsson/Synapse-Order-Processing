using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using Newtonsoft.Json.Linq;
using Synapse.OrderProcessing.Core.Interfaces;
using Synapse.OrderProcessing.Core.Services;
using System.Net;

namespace Synapse.OrderProcessing.Core.UnitTests.Utilities
{
    public static class TestUtilities
    {
        public static JToken BuildJToken(string? item = null)
        {
            string json = item ?? @"{ }";
            JToken returnValue = JToken.Parse(json);
            return returnValue;
        }

        public static INotificationService BuildNotificationService(Mock<HttpMessageHandler> httpMessageHandlerMock, 
                                                                    Mock<IConfiguration> configurationMock,
                                                                    HttpClient httpClient,
                                                                    bool isSuccess = true) {
            // Mocking IConfiguration
            configurationMock.Setup(config => config["ApiSettings:Alerts:Url"])
                              .Returns("https://mockapi.com/alerts");
            configurationMock.Setup(config => config["ApiSettings:Alerts:MaxRetryAttempts"])
                              .Returns("3");
            configurationMock.Setup(config => config["ApiSettings:Alerts:DelayBetweenRetriesMilliseconds"])
                              .Returns("1000");

            // Mocking HttpMessageHandler to return a successful response
            httpMessageHandlerMock
                .Protected() // Accesses the protected members of HttpMessageHandler
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync", // The name of the method
                    ItExpr.IsAny<HttpRequestMessage>(), // Any HttpRequestMessage
                    ItExpr.IsAny<CancellationToken>()) // Any CancellationToken
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = isSuccess ? HttpStatusCode.OK : HttpStatusCode.InternalServerError
                });

            // Creating HttpClient with mocked handler
            httpClient = new HttpClient(httpMessageHandlerMock.Object);

            // Creating instance of NotificationService with mocked dependencies
            return new NotificationService(httpClient, configurationMock.Object);
        }
        public static JObject[] LoadTestData(string pathToData)
        {
            if (File.Exists(pathToData)) {
                string json = File.ReadAllText(pathToData);
                return JArray.Parse(json)?.ToObject<JObject[]>() ?? new JObject[1];
            }
            return new JObject[1];
        }

        public static string BuildJTokenString(string orderId, string description, string status, int deliveryNotification)
        {
            return $@"{{ ""OrderId"": ""{orderId}"", ""Description"": ""{description}"", ""Status"": ""{status}"", ""DeliveryNotification"": {deliveryNotification} }}";
        }
    }
}
