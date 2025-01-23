using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PromptEngineering.Models;
using PromptEngineering.Services;
using System.Text.Json;
using System.Threading.Tasks;
using System;

namespace PromptEngineering.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly ILogger<FilesController> _logger;
        private readonly IFilesServices _filesServices;

        public FilesController(ILogger<FilesController> logger, IFilesServices filesServices)
        {
            _logger = logger;
            _filesServices = filesServices;
        }

        [HttpPost]
        [Route("upload")]
        [Produces("application/json")]
        public async Task<IActionResult> UploadFile(IFormFile fileInput)
        {
            _logger.LogInformation($"UploadFile request received at {DateTime.Now}");
            _logger.LogInformation("UploadFile request Inputs at {0}", JsonSerializer.Serialize(fileInput));

            if (fileInput == null || fileInput.Length == 0)
            {
                return BadRequest("File is Empty");
            }

            var response = await _filesServices.UploadFile(fileInput);
            if (response == null)
            {
                return NotFound();
            }
            return Ok(new { FileName = response });
        }
        
    }
}
