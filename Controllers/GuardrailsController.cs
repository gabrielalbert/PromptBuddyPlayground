using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PromptEngineering.Models;
using PromptEngineering.Services;
using System.Threading.Tasks;
using System;

namespace PromptEngineering.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GuardrailsController : ControllerBase
    {
        private readonly IGuardrailsServices _guardrailsServices;
        private readonly ILogger<GuardrailsController> _logger;
        public GuardrailsController(ILogger<GuardrailsController> logger, IGuardrailsServices guardrailsServices)
        {
            _logger = logger;
            _guardrailsServices = guardrailsServices;
        }

        [HttpGet]
        [Route("list")]
        [Produces("application/json")]
        public async Task<IActionResult> GetGuardrails()
        {
            _logger.LogInformation($"GetGuardrails request received at {DateTime.Now}");

            var response = await _guardrailsServices.GetAll();
            if (response == null)
            {
                return NotFound();
            }
            return Ok(response);
        }

        [HttpPost]
        [Route("add")]
        [Produces("application/json")]
        public async Task<IActionResult> AddGuardRail(GuardrailModel guardrailModel)
        {
            _logger.LogInformation($"AddGuardrail request received at {DateTime.Now}");

            var response = await _guardrailsServices.AddGuardrail(guardrailModel);
            if (response == 0)
            {
                return NotFound();
            }
            return Ok(response);
        }

        [HttpPost]
        [Route("save")]
        [Produces("application/json")]
        public async Task<IActionResult> UpdateGuardrail(GuardrailModel guardrailModel)
        {
            _logger.LogInformation($"UpdateGuardrail request received at {DateTime.Now}");

            var response = await _guardrailsServices.UpdateGuardrail(guardrailModel);
            if (response == 0)
            {
                return NotFound();
            }
            return Ok(response);
        }

        [HttpDelete]
        [Route("delete/{id}")]
        [Produces("application/json")]
        public async Task<IActionResult> DeleteGuardrail(int id)
        {
            _logger.LogInformation($"DeleteGuardrail request received at {DateTime.Now}");

            var response = await _guardrailsServices.DeleteGuardrail(id);
            if (response == 0)
            {
                return NotFound();
            }
            return Ok(response);
        }


    }
}
