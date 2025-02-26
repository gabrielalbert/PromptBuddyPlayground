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
    public class DocsController : ControllerBase
    {
        private readonly ILogger<DocsController> _logger;
        private readonly IDocsServices _docsServices;

        public DocsController(ILogger<DocsController> logger, IDocsServices docsServices)
        {
            _logger = logger;
            _docsServices = docsServices;
        }


        [HttpGet]
        [Route("video-ppt")]
        [Produces("application/json")]
        public async Task<ActionResult> GetKTDocsSummary()
        {
            _logger.LogInformation($"GetKTDocsSummary Get request received at {DateTime.Now}");

            var docsSummary = await _docsServices.GetKTDocsSummary();
            return Ok(docsSummary);
        }
    }
}
