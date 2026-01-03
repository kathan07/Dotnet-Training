using MCP_POC.Business.MCP;
using Microsoft.AspNetCore.Mvc;

namespace MCP_POC.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMcpManager _mcpManager;

        public HomeController(ILogger<HomeController> logger, IMcpManager mcpManager)
        {
            _logger = logger;
            _mcpManager = mcpManager;
        }

        [HttpPost]
        public async Task<IActionResult> GetTools()
        {
            var response = await _mcpManager.GetTools();
            return Ok(response);
        }
    }
}
