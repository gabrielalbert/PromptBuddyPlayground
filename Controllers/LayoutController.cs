using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PromptEngineering.Services;
using System.Text.Json;
using System.Threading.Tasks;
using System;
using PromptEngineering.Models;
using System.Reflection.Metadata;
using System.Text;

namespace PromptEngineering.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LayoutController : ControllerBase
    {
        private readonly ILogger<LayoutController> _logger;
        private readonly ILayoutServices _layoutServices;

        public LayoutController(ILogger<LayoutController> logger, ILayoutServices layoutServices)
        {
            _logger = logger;
            _layoutServices = layoutServices;
        }

        [HttpPost]
        [Route("upload-image")]
        [Produces("application/json")]
        public async Task<IActionResult> UploadImageFile(LayoutModel input)
        {
            _logger.LogInformation($"UploadFile request received at {DateTime.Now}");
            _logger.LogInformation("UploadFile request Inputs at {0}", JsonSerializer.Serialize(input));

            if ( input==null || string.IsNullOrEmpty(input.ImageFile))
            {
                return BadRequest("No image file uploaded");
            }

            var response = await _layoutServices.AddUiLayoutEntry(input);

            if (response == null)
            {
                return NotFound();
            }
            return Ok(response);// new { FileName = response });
        }

        [HttpGet]
        [Route("list-all")]
        [Produces("application/json")]
        public async Task<IActionResult> GetAllUiLayoutDatas()
        {
            _logger.LogInformation($"GetAllUiLayoutDatas Get request received at {DateTime.Now}");

            var result = await _layoutServices.GetAllUiLayoutDatas();
            return Ok(result);
        }

        [HttpGet]
        [Route("{layoutId}/start-process")]
        [Produces("application/json")]
        public async Task<IActionResult> StartProcess([FromRoute] int layoutId,[FromHeader] string userName)
        {
            _logger.LogInformation($"StartProcess request received at {DateTime.Now}");
            _logger.LogInformation("StartProcess request Inputs at {0} {1}", layoutId,userName);

            var response = await _layoutServices.StartProcess(layoutId,userName);
            
            return Ok(response);// new { FileName = response });
        }

        [HttpGet]
        [Route("{layoutId}/download")]
        [Produces("application/json")]
        public async Task<IActionResult> DownloadFile(int layoutId)
        {
            _logger.LogInformation($"DownloadFile request received at {DateTime.Now}");
            _logger.LogInformation("DownloadFile request Inputs at {0}", layoutId);

            var (responseContent,filename) = await _layoutServices.DownloadFile(layoutId);
            if (responseContent == null)
            {
                return NotFound();
            }
            byte[] fileBytes = Encoding.UTF8.GetBytes(responseContent);
            // Return the file for download
            return File(fileBytes, "text/plain", filename);            
        }

        [HttpGet]
        [Route("{layoutId}/view-image")]
        [Produces("application/json")]
        public async Task<IActionResult> GetImageContent(int layoutId)
        {
            _logger.LogInformation($"GetImageContent request received at {DateTime.Now}");
            _logger.LogInformation("GetImageContent request Inputs at {0}", layoutId);

            var response = await _layoutServices.GetImageContent(layoutId);
            if (response == null)
            {
                return NotFound();
            }
            return Ok(response);
        }


    }
}
