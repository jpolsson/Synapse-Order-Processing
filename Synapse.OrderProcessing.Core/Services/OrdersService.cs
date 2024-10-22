using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Serilog;
using Synapse.OrderProcessing.Core.Interfaces;
using Synapse.OrderProcessing.Core.Models;


namespace Synapse.OrderProcessing.Core.Services
{
    public class OrdersService : IOrdersService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly string _fetchApiUrl;
        private readonly string _updateApiUrl;
        private readonly int _maxRetryAttempts;
        private readonly int _delayBetweenRetries;

        public OrdersService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            // Read settings from configuration
            _updateApiUrl = _configuration["ApiSettings:OrdersUpdate:Url"] ?? string.Empty;
            _fetchApiUrl = _configuration["ApiSettings:OrdersFetch:Url"] ?? string.Empty;
            _maxRetryAttempts = int.Parse(_configuration["ApiSettings:OrdersUpdate:MaxRetryAttempts"] ?? "3");
            _delayBetweenRetries = int.Parse(_configuration["ApiSettings:OrdersUpdate:DelayBetweenRetriesMilliseconds"] ?? "1000");
        }
        public async Task<List<EquipmentOrder>> GetOrders()
        {
            var response = await _httpClient.GetAsync(_updateApiUrl);
            if (response.IsSuccessStatusCode)
            {
                var ordersData = await response.Content.ReadAsStringAsync();
                return JArray.Parse(ordersData ?? "[]")
                    .ToObject<JObject[]>()?
                    .Select(jObj => new EquipmentOrder(jObj))
                    .ToList() ?? new List<EquipmentOrder>();
            }
            else
            {
                Console.WriteLine("Failed to fetch orders from API.");
                return new List<EquipmentOrder>();
            }
        }

        public async Task<bool> UpdateOrder(JObject order)
        {
            int retryCount = 0;
            while (true)
            {
                try
                {
                    var content = new StringContent(order.ToString(), System.Text.Encoding.UTF8, "application/json");
                    var response = await _httpClient.PostAsync(_updateApiUrl, content);
                    // If the request is successful, return the content
                    if (response.IsSuccessStatusCode)
                    {
                        return true;
                    }

                    // If not successful, throw an exception to trigger retry logic
                    response.EnsureSuccessStatusCode();
                }
                catch (Exception ex) when (retryCount < _maxRetryAttempts)
                {
                    retryCount++;
                    Log.Information($"Request failed. Attempt {retryCount} of {_maxRetryAttempts}. Retrying in {_delayBetweenRetries} ms...");
                    await Task.Delay(_delayBetweenRetries);
                }
                catch(Exception ex)
                {
                    Log.Error($"Unexpected Error in UpdateOrder:{ex.Message}", ex);
                    return false;
                }
            }
        }
    }
}
