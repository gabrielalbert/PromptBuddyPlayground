using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace PromptEngineering.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CodeController : ControllerBase
    {
        //[HttpGet]
        //[Route("llm-models")]
        //[Produces("application/json")]
        //public IActionResult GetLlmModels()
        //{
        //    //_logger.LogInformation($"GetLlmModels request received at {DateTime.Now}");

        //    //var response = _masterServices.GetAiModels();
        //    //if (response == null)
        //    //{
        //    //    return NotFound();
        //    //}
        //    return Ok(response);
        //}
    }
}
