using Microsoft.AspNetCore.Mvc;
using Synapse.MockServer.Models;

namespace Synapse.MockServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlertsController : ControllerBase
    {
        [HttpPost]
        public IActionResult SendAlert([FromBody] AlertMessage alertMessage)
        {
            return Ok();
        }
    }
}
