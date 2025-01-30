using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PromptEngineering.Services;
using System.Text.Json;
using System.Threading.Tasks;
using System;

namespace PromptEngineering.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RepoController : ControllerBase
    {
        private readonly ILogger<RepoController> _logger;
        private readonly IRepoServices _repoServices;

        public RepoController(ILogger<RepoController> logger, IRepoServices repoServices)
        {
            _logger = logger;
            _repoServices = repoServices;
        }

        [HttpPost]
        [Route("uploadfiles")]
        [Produces("application/json")]
        public async Task<IActionResult> UploadFiles([FromForm] IFormFile[] files, [FromForm] string repoName)
        {
            _logger.LogInformation($"UploadFile request received at {DateTime.Now}");
            _logger.LogInformation("UploadFile request Inputs at {0}", JsonSerializer.Serialize(files));

            if (files == null || files.Length == 0)
            {
                return BadRequest("Folder is Empty");
            }

            await _repoServices.AddRepoFiles(files, repoName);

            //if (response == null)
            //{
            //    return NotFound();
            //}
            return Ok();// new { FileName = response });
        }

        [HttpGet]
        [Route("list")]
        [Produces("application/json")]
        public async Task<IActionResult> GetAllRepos()
        {
            _logger.LogInformation($"GetAllRepos Get request received at {DateTime.Now}");

            var result = await _repoServices.GetAllRepos();
            return Ok(result);
        }

        [HttpGet]
        [Route("summary")]
        [Produces("application/json")]
        public async Task<ActionResult> GetRepoSummary()
        {
            _logger.LogInformation($"GetRepoSummary Get request received at {DateTime.Now}");

            var repoSummary = await _repoServices.GetRepoSummary();
            return Ok(repoSummary);
        }

        [HttpGet]
        [Route("{repoName}/files")]
        [Produces("application/json")]
        public async Task<ActionResult> GetRepoFiles(string repoName)
        {
            _logger.LogInformation($"GetFilesByRepo Get request received at {DateTime.Now}");

            var files = await _repoServices.GetRepoFiles(repoName);
            return Ok(files);
        }
    }
}
