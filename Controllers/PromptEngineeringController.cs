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

        [HttpPost]
        [Route("message")]
        [Produces("application/json")]
        public async Task<IActionResult> GetChatMessage([FromBody] ChatInput input)
        {
            _logger.LogInformation($"GetChatMessage request received at {DateTime.Now}");
            _logger.LogInformation("GetChatMessage request Inputs at {0}", JsonSerializer.Serialize(input));

            var response = await _chatServices.GetChatMessage(input);
            if (response == null)
            {
                return NotFound();
            }
            return Ok(response);
        }

        
        [HttpGet]
        [Route("messages")]
        [Produces("application/json")]
        public async Task<IActionResult> GetChatMessages([FromHeader] string userName, [FromHeader] string startDate = "", [FromHeader] string endDate="")
        {
            _logger.LogInformation($"GetChatMessages Get request received at {DateTime.Now}");
            _logger.LogInformation($"GetMessages Controller Input {userName} {startDate} {endDate}");

            var chatMessages = await _chatServices.GetAllChatMessages(userName, startDate,endDate);
            if (chatMessages == null)
            {
                return NotFound();
            }
            return Ok(chatMessages);
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

        [HttpGet]
        [Route("autocomplete")]
        [Produces("application/json")]
        public async Task<IActionResult> GetAutoCompleteSuggestions([FromHeader] string aiModel, [FromQuery] string query)
        {
            _logger.LogInformation($"GetAutoCompleteSuggestions Get request received at {DateTime.Now}");

            if (string.IsNullOrEmpty(query))
            {
                return BadRequest("Query parameter is required.");
            }
            var suggestions = await _chatServices.GetAutoCompleteSuggestions(aiModel, query);           
            return Ok(suggestions);
        }

        [HttpPut]
        [Route("feedback/{feedback}/{chatId}")]
        [Produces("application/json")]
        public async Task<IActionResult> UpdateFeedback(string chatId, string feedback)
        {
            _logger.LogInformation($"UpdateFeedback Post request received at {DateTime.Now}");
            _logger.LogInformation($"UpdateFeedback Controller Input {chatId} {feedback}");

            if (string.IsNullOrEmpty(feedback))
            {
                return BadRequest("Feedback parameter is required.");
            }
            await _chatServices.UpdateFeedback(Convert.ToInt32(chatId), feedback);
            
            return Ok();
        }
    }

}