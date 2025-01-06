using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PromptEngineering.Services;
using System;

namespace PromptEngineering.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private readonly IUsersServices _usersServices;

        public UsersController(ILogger<UsersController> logger, IUsersServices usersServices)
        {
            _logger = logger;
            _usersServices = usersServices;
        }

        [HttpGet]
        [Route("read")]
        [Produces("application/json")]
        public IActionResult GetUsers()
        {
            _logger.LogInformation($"GetUsers request received at {DateTime.Now}");

            var response = _usersServices.GetUsers();
            if (response == null)
            {
                return NotFound();
            }
            return Ok(response);
        }
    }
}
