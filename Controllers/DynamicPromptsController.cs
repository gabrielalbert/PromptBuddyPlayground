using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PromptEngineering.Data;
using PromptEngineering.Data.ApplicationDBEntity;
using PromptEngineering.Models.DynamicPrompt;
using PromptEngineering.Services.CodeConversion;

namespace ConversionServer.Controllers
{
    [ApiController]
    [Route("api/dynamicprompts")]
    public class DynamicPromptsController : ControllerBase
    {
        private IApplicationDBContext ApplicationDBContext { get; set; }

        public DynamicPromptsController(
            IApplicationDBContext applicationDBContext
        )
        {
            ApplicationDBContext = applicationDBContext;
        }

        [HttpPost]
        [Route("addUpdate")]
        public async Task<DynamicPromptAddResponseModel> AddUpdateDynamicPrompt(DynamicPromptModel dynamicPromptModel)
        {
            var responseModel = await ValidateDynamicPromptModel(dynamicPromptModel);
            if (!responseModel.Success)
            {
                return responseModel;
            }
            var success = await ApplicationDBContext.AddUpdateDynamicPrompt(new DynamicPrompt
            {
                DynamicPromptId = dynamicPromptModel.DynamicPromptId,
                UniqueName = dynamicPromptModel.UniqueName,
                Description = dynamicPromptModel.Description,
                SystemPromptTemplate = dynamicPromptModel.SystemPromptTemplate,
                UserPromptTemplate = dynamicPromptModel.UserPromptTemplate,
                Temperature = dynamicPromptModel.Temperature,
                Seed = dynamicPromptModel.Seed,
                ModelName = dynamicPromptModel.ModelName,
                SystemPromptKeysCommaSeperated = string.Join(",", dynamicPromptModel.SystemPromptKeys ?? []),
                UserPromptKeysCommaSeperated = string.Join(",", dynamicPromptModel.UserPromptKeys ?? [])
            });
            return new DynamicPromptAddResponseModel
            {
                Success = true,
                ErrorMessages = new List<string>()
            };
        }

        [HttpGet]
        [Route("getAllDynamicPrompts")]
        public async Task<List<DynamicPromptListModel>> GetAllDynamicPrompts()
        {
            return await (await ApplicationDBContext.GetAllDynamicPrompts()).Select(e => new DynamicPromptListModel
            {
                DynamicPromptId = e.DynamicPromptId,
                UniqueName = e.UniqueName,
                Description = e.Description
            }).ToListAsync();
        }

        [HttpGet]
        [Route("getDynamicPromptFromId")]
        public async Task<DynamicPromptModel> GetDynamicPromptModelFromId(int dynamicPromptId)
        {
            var dynamicPrompt = await ApplicationDBContext.GetDynamicPrompt(dynamicPromptId);
            return ConvertDynamicPromptToModel(dynamicPrompt);
        }

        [HttpGet]
        [Route("getDynamicPromptFromUniqueName")]
        public async Task<DynamicPromptModel> GetDynamicPromptModelFromUniqueName(string uniqueName)
        {
            var dynamicPrompt = await ApplicationDBContext.GetDynamicPrompt(uniqueName);
            return ConvertDynamicPromptToModel(dynamicPrompt);
        }

        private async Task<DynamicPromptAddResponseModel> ValidateDynamicPromptModel(DynamicPromptModel dynamicPromptModel)
        {
            var errorMessages = new List<string>();
            if (dynamicPromptModel.UniqueName == null || dynamicPromptModel.UniqueName.Length < 3)
            {
                errorMessages.Add("Name is too short");
            }
            else
            {
                var idUniqueNames = (await ApplicationDBContext.GetAllDynamicPrompts()).Select(e => new { id = e.DynamicPromptId, uniqueName = e.UniqueName }).ToList();
                var existingModel = idUniqueNames.FirstOrDefault(e => e.id == dynamicPromptModel.DynamicPromptId);
                var allowedUniqueNames = new List<string>();
                if (existingModel != null)
                {
                    allowedUniqueNames.Add(existingModel.uniqueName.ToLower());
                }
                var nameIsUnique = idUniqueNames.FirstOrDefault(e => e.uniqueName.ToLower() == dynamicPromptModel.UniqueName.ToLower()) == null;
                if (!nameIsUnique && !allowedUniqueNames.Contains(dynamicPromptModel.UniqueName.ToLower()))
                {
                    errorMessages.Add("Name is not unique");
                }
            }
            if (dynamicPromptModel.ModelName == null || dynamicPromptModel.ModelName.Length < 3)
            {
                errorMessages.Add("Model name is too short");
            }
            if (dynamicPromptModel.Temperature != null && (dynamicPromptModel.Temperature < 0 || dynamicPromptModel.Temperature > 100))
            {
                errorMessages.Add("Invalid Temperature value");
            }
            if (dynamicPromptModel.Seed != null && dynamicPromptModel.Seed < 0)
            {
                errorMessages.Add("Invalid Seed value");
            }
            (var _, var userPromptValidateErrorMessages) = DynamicPromptTemplateProcessor.IsPromptTemplateStringValid(dynamicPromptModel.UserPromptTemplate, dynamicPromptModel.UserPromptKeys ?? []);
            (var _, var systemPromptValidateErrorMessages) = DynamicPromptTemplateProcessor.IsPromptTemplateStringValid(dynamicPromptModel.SystemPromptTemplate, dynamicPromptModel.SystemPromptKeys ?? []);

            errorMessages.AddRange(userPromptValidateErrorMessages);
            errorMessages.AddRange(systemPromptValidateErrorMessages);

            return new DynamicPromptAddResponseModel
            {
                Success = errorMessages.Count() == 0,
                ErrorMessages = errorMessages
            };
        }

        private DynamicPromptModel ConvertDynamicPromptToModel(DynamicPrompt dynamicPrompt)
        {
            return new DynamicPromptModel
            {
                DynamicPromptId = dynamicPrompt.DynamicPromptId,
                UniqueName = dynamicPrompt.UniqueName,
                Description = dynamicPrompt.Description,
                SystemPromptTemplate = dynamicPrompt.SystemPromptTemplate,
                UserPromptTemplate = dynamicPrompt.UserPromptTemplate,
                Temperature = dynamicPrompt.Temperature,
                Seed = dynamicPrompt.Seed,
                ModelName = dynamicPrompt.ModelName,
                SystemPromptKeys = DynamicPromptTemplateProcessor.GetArrayFromCommaSeperatedKeys(dynamicPrompt.SystemPromptKeysCommaSeperated),
                UserPromptKeys =DynamicPromptTemplateProcessor.GetArrayFromCommaSeperatedKeys(dynamicPrompt.UserPromptKeysCommaSeperated)
            };
        }
    }
}