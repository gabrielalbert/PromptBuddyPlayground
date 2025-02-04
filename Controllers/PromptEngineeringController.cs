using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PromptEngineering.Models;
using PromptEngineering.Services;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace PromptEngineering.Controllers
{
    [Route("api/[controller]")]
    public class PromptEngineeringController : ControllerBase
    {
        private readonly ILogger<PromptEngineeringController> _logger;
        private readonly IChatServices _chatServices;

        public PromptEngineeringController(ILogger<PromptEngineeringController> logger, IChatServices chatServices)
        {
            _logger = logger;
            _chatServices = chatServices;
        }        

        [HttpGet]
        [Route("health")]
        [Produces("application/json")]
        public async Task<IActionResult> HealthCheck()
        {
            _logger.LogInformation($"HealthCheck request received at {DateTime.Now}");
            _logger.LogInformation($"HealthCheck GET request received at {DateTime.Now}");

            return Ok(new { reply = $"API GET is reachable" });
        }

        [HttpGet]
        [Route("dashboard")]
        [Produces("application/json")]
        public async Task<IActionResult> GetDashboardInfo()
        {
            _logger.LogInformation($"GetDashboard Get request received at {DateTime.Now}");

            var result = await _chatServices.GetNumbersOfPromptEnggTypes();
            return Ok(result);
        }
        
    }

}