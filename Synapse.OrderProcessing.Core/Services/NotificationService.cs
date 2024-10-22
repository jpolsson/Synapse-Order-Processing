using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Serilog;
using Synapse.OrderProcessing.Core.Interfaces;
using Synapse.OrderProcessing.Core.Models;


namespace  Synapse.OrderProcessing.Core.Services
{
    public class NotificationService : INotificationService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        public NotificationService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }
        public async Task<bool> SendNotification(EquipmentOrderItem item)
        {
            // Read settings from configuration
            string url = _configuration["ApiSettings:Alerts:Url"] ?? string.Empty;
            int maxRetryAttempts = int.Parse(_configuration["ApiSettings:Alerts:MaxRetryAttempts"] ?? "3");
            int delayBetweenRetries = int.Parse(_configuration["ApiSettings:Alerts:DelayBetweenRetriesMilliseconds"] ?? "1000");

            var alertMessage = BuildDeliveredItemAlert(item);
            int retryCount = 1;
            while (true)
            {
                try
                {
                    var alertData = new { message = alertMessage };
                    var content = new StringContent(JObject.FromObject(alertData).ToString(), System.Text.Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await _httpClient.PostAsync(url, content);
                    if (response.IsSuccessStatusCode)
                    {
                        return true;
                    }

                    // If not successful, throw an exception to trigger retry logic
                    response.EnsureSuccessStatusCode();
                }
                catch (Exception ex) when (retryCount < maxRetryAttempts)
                {
                    retryCount++;
                    Log.Information($"Request failed for alert: {alertMessage}. Attempt {retryCount} of {maxRetryAttempts}. Retrying in {delayBetweenRetries} ms...");
                    await Task.Delay(delayBetweenRetries);
                }
                catch(Exception ex)
                {
                    Log.Error($"Unexpected Error in SendNotification:{ex.Message}", ex);
                    // If all retries fail, return false
                    return false;
                }
            }
        }

        private string BuildDeliveredItemAlert(EquipmentOrderItem item)
        {
            return $"Alert for delivered item: Order {item.OrderId},Item: {item.Description},Delivery Notifications: {item.DeliveryNotification}";
        }
    }
}
