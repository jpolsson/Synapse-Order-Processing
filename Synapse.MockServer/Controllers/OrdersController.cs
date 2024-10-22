using Microsoft.AspNetCore.Mvc;
using Synapse.MockServer.Models;


namespace Synapse.MockServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly string _dataFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..","Synapse.OrderProcessing.Core.UnitTests", "Utilities", "TestOrderData.json");
        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            if (!System.IO.File.Exists(_dataFilePath))
            {
                return NotFound("Data file not found.");
            }

            // Read JSON data from the file
            var jsonData = await System.IO.File.ReadAllTextAsync(_dataFilePath);
            

            return Ok(jsonData);
        }

        [HttpPost]
        public IActionResult SendAlert([FromBody] OrderUpdate orderUpdate)
        {
            return Ok();
        }
    }
}
