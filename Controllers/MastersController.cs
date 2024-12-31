using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PromptEngineering.Services;
using System;

namespace PromptEngineering.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MastersController : ControllerBase
    {
        private readonly ILogger<MastersController> _logger;
        private readonly IMasterServices _masterServices;

        public MastersController(ILogger<MastersController> logger, IMasterServices masterServices)
        {
            _logger = logger;
            _masterServices = masterServices;
        }

        [HttpGet]
        [Route("proglangs")]
        [Produces("application/json")]
        public IActionResult GetProgLangs()
        {
            _logger.LogInformation($"GetProgLang request received at {DateTime.Now}");

            var response = _masterServices.GetProgLangs();
            if (response == null)
            {
                return NotFound();
            }
            return Ok(response);
        }

        [HttpGet]
        [Route("offerings")]
        [Produces("application/json")]
        public IActionResult GetOfferings()
        {
            _logger.LogInformation($"GetOfferings request received at {DateTime.Now}");

            var response = _masterServices.GetOfferings();
            if (response == null)
            {
                return NotFound();
            }
            return Ok(response);
        }

        [HttpGet]
        [Route("phases")]
        [Produces("application/json")]
        public IActionResult GetPhases()
        {
            _logger.LogInformation($"GetPhases request received at {DateTime.Now}");

            var response = _masterServices.GetPhases();
            if (response == null)
            {
                return NotFound();
            }
            return Ok(response);
        }

        [HttpGet]
        [Route("roles")]
        [Produces("application/json")]
        public IActionResult GetRoles()
        {
            _logger.LogInformation($"GetRoles request received at {DateTime.Now}");

            var response = _masterServices.GetRoles();
            if (response == null)
            {
                return NotFound();
            }
            return Ok(response);
        }

        [HttpGet]
        [Route("users")]
        [Produces("application/json")]
        public IActionResult GetUsers()
        {
            _logger.LogInformation($"GetUsers request received at {DateTime.Now}");

            var response = _masterServices.GetUsers();
            if (response == null)
            {
                return NotFound();
            }
            return Ok(response);
        }
    }
}
