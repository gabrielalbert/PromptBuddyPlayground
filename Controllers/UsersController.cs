using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PromptEngineering.Models;
using PromptEngineering.Services;
using System;
using System.Threading.Tasks;

namespace PromptEngineering.Controllers
{
    [Route("api/user")]
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
        [Route("list")]
        [Produces("application/json")]
        public async Task<IActionResult> GetUsers()
        {
            _logger.LogInformation($"GetUsers request received at {DateTime.Now}");

            var response = await _usersServices.GetUsers();
            if (response == null)
            {
                return NotFound();
            }
            return Ok(response);
        }

        [HttpPost]
        [Route("create")]
        [Produces("application/json")]
        public async Task<IActionResult> CreateUser(UserViewModel user)
        {
            _logger.LogInformation($"CreateUser request received at {DateTime.Now}");

            var response = await _usersServices.CreateUser(user);
            if (response == null)
            {
                return NotFound();
            }
            return Ok(response);
        }

        [HttpPost]
        [Route("update")]
        [Produces("application/json")]
        public async Task<IActionResult> UpdateUser(UserViewModel user)
        {
            _logger.LogInformation($"UpdateUser request received at {DateTime.Now}");

            var response = await _usersServices.UpdateUser(user);
            if (response == null)
            {
                return NotFound();
            }
            return Ok(response);
        }

        [HttpDelete]
        [Route("delete/{userId}")]
        [Produces("application/json")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            _logger.LogInformation($"DeleteUser request received at {DateTime.Now}");

            var response = await _usersServices.DeleteUser(userId);
            if (response == null)
            {
                return NotFound();
            }
            return Ok(response);
        }

        [HttpPut]
        [Route("{userId}/change-password")]
        [Produces("application/json")]
        public async Task<IActionResult> UpdatePassword(int userId, string password)
        {
            _logger.LogInformation($"UpdatePassword request received at {DateTime.Now}");

            var response = await _usersServices.ChangePassword(userId,password);
            if (response == null)
            {
                return NotFound();
            }
            return Ok(response);
        }

        [HttpGet]
        [Route("validate")]
        [Produces("application/json")]
        public async Task<IActionResult> ValidateUser(string userName)
        {
            _logger.LogInformation($"ValidateUser request received at {DateTime.Now}");

            var response = await _usersServices.ValidateUser(userName);
            if (response == null)
            {
                return NotFound();
            }
            return Ok(response);
        }
        

        [HttpGet]
        [Route("{userId}/managers")]
        [Produces("application/json")]
        public async Task<IActionResult> GetReportingToUsers(int userId)
        {
            _logger.LogInformation($"GetReportingToUsers request received at {DateTime.Now}");

            var response =await _usersServices.GetReportingToUsers(userId);
            if (response == null)
            {
                return NotFound();
            }
            return Ok(response);
        }

        [HttpPut]
        [Route("{userId}/update-manager")]
        [Produces("application/json")]
        public async Task<IActionResult> UpdateManagers(int userId, int managerId)
        {
            _logger.LogInformation($"UpdateManagers request received at {DateTime.Now}");

            var response = await _usersServices.UpdateManagers(userId,managerId);
            if (response == null)
            {
                return NotFound();
            }
            return Ok(response);
        }

        [HttpGet]
        [Route("{userId}")]
        [Produces("application/json")]
        public async Task<IActionResult> GetUser(int userId)
        {
            _logger.LogInformation($"GetUser request received at {DateTime.Now}");

            var response = await _usersServices.GetUser(userId);
            if (response == null)
            {
                return NotFound();
            }
            return Ok(response);
        }

        [HttpGet]
        [Route("login")]
        [Produces("application/json")]
        public async Task<IActionResult> LoginUser(string userName, string password)
        {
            _logger.LogInformation($"LoginUser request received at {DateTime.Now}");

            var response = await _usersServices.LoginUser(userName, password);
            if (response == null)
            {
                return NotFound();
            }
            return Ok(response);
        }

    }
}
