using Microsoft.AspNetCore.Http;
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
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly ILogger<ChatController> _logger;
        private readonly IChatServices _chatServices;


        public ChatController(ILogger<ChatController> logger, IChatServices chatServices)
        {
            _logger = logger;
            _chatServices = chatServices;
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
        public async Task<IActionResult> GetChatMessages([FromHeader] string userName, [FromHeader] string startDate = "", [FromHeader] string endDate = "", [FromHeader] string aiModel = "")
        {
            _logger.LogInformation($"GetChatMessages Get request received at {DateTime.Now}");
            _logger.LogInformation($"GetMessages Controller Input {userName} {startDate} {endDate} {aiModel}");

            var chatMessages = await _chatServices.GetAllChatMessages(userName, aiModel, startDate, endDate);
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

        [HttpPost]
        [Route("ai-assistant")]
        [Produces("application/json")]
        public async Task<IActionResult> GetChatMessageUpdated([FromBody] AiAssistantInput newInput)
        {
            _logger.LogInformation($"GetChatMessageUpdated request received at {DateTime.Now}");
            _logger.LogInformation("GetChatMessageUpdated request Inputs at {0}", JsonSerializer.Serialize(newInput));

            var response = await _chatServices.GetNewChatMessage(newInput);
            if (response == null)
            {
                return NotFound();
            }
            return Ok(response);
        }

        [HttpGet]
        [Route("chat-groups")]
        [Produces("application/json")]
        public async Task<IActionResult> GetRecentChats()
        {
            _logger.LogInformation($"GetRecentChats get request received at {DateTime.Now}");

            var recentChats = await _chatServices.GetRecentChatsAsync();
            if (recentChats == null)
            {
                return NotFound();
            }
            return Ok(recentChats);
        }

        [HttpGet]
        [Route("{groupId}/chat-messages")]
        [Produces("application/json")]
        public async Task<IActionResult> GetChatMessagesByGroupID([FromRoute] int groupId, [FromHeader] string startDate = "", [FromHeader] string endDate = "", [FromHeader] string aiModel = "")
        {
            _logger.LogInformation($"GetChatMessagesByGroupID Get request received at {DateTime.Now}");
            _logger.LogInformation($"GetChatMessagesByGroupID Controller Input {groupId} {startDate} {endDate} {aiModel}");

            var chatMessages = await _chatServices.GetAllChatMessagesByGroupID(groupId, aiModel, startDate, endDate);
            if (chatMessages == null)
            {
                return NotFound();
            }
            return Ok(chatMessages);
        }


    }
}
