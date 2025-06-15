using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//using PromptEngineering.Data;
using PromptEngineering.Models.DynamicPrompt;

namespace PromptEngineering.Services.CodeConversion
{
    public class DynamicPromptService : IDynamicPromptService
    {
        //private IApplicationDBContext ApplicationDBContext;

        public DynamicPromptService()
        {
            //this.ApplicationDBContext = applicationDBContext;
        }

        public async Task<LLMPrompt> GetDynamicPrompt(string promptKey, Dictionary<string, string> userKeyValues, Dictionary<string, string> systemKeyValues)
        {
            //var dynamicPrompt = new List<DynamicPromptModel>();// await ApplicationDBContext.GetDynamicPrompt(promptKey);
            //var userKeys = DynamicPromptTemplateProcessor.GetArrayFromCommaSeperatedKeys(dynamicPrompt.UserPromptKeysCommaSeperated);
            //var systemKeys = DynamicPromptTemplateProcessor.GetArrayFromCommaSeperatedKeys(dynamicPrompt.SystemPromptKeysCommaSeperated);

            //(var userTemplateValid, var userErrorMessages) = DynamicPromptTemplateProcessor.IsPromptTemplateStringValid(dynamicPrompt.UserPromptTemplate, userKeys);
            //if (!userTemplateValid)
            //{
            //    throw new Exception($"Fix template! User template with key '{promptKey}' is invalid due to: \n {string.Join("\n", userErrorMessages)}");
            //}

            //(var systemTemplateValid, var systemErrorMessages) = DynamicPromptTemplateProcessor.IsPromptTemplateStringValid(dynamicPrompt.SystemPromptTemplate, systemKeys);
            //if (!systemTemplateValid)
            //{
            //    throw new Exception($"Fix template! System template with key '{promptKey}' is invalid due to: \n {string.Join("\n", systemErrorMessages)}");
            //}

            //var inputUserKeys = userKeyValues.Keys.ToList();
            //var inputSystemKeys = systemKeyValues.Keys.ToList();

            //(var userKeysValid, var userKeysErrorMessages) = DynamicPromptTemplateProcessor.IsPromptTemplateStringValid(dynamicPrompt.UserPromptTemplate, inputUserKeys);
            //if (!userKeysValid)
            //{
            //    throw new Exception($"Fix user input dictionary! Dictionary input for key '{promptKey}' is invalid due to: \n {string.Join("\n", userKeysErrorMessages)}");
            //}

            //(var systemKeysValid, var systemKeysErrorMessages) = DynamicPromptTemplateProcessor.IsPromptTemplateStringValid(dynamicPrompt.SystemPromptTemplate, inputSystemKeys);
            //if (!systemKeysValid)
            //{
            //    throw new Exception($"Fix system input dictionary! Dictionary input for key {promptKey} is invalid due to: \n {string.Join("\n", systemKeysErrorMessages)}");
            //}

            return new LLMPrompt();
            //{
            //    ModelName = dynamicPrompt.ModelName,
            //    UserPrompt = DynamicPromptTemplateProcessor.GetPromptStringFromTemplate(dynamicPrompt.UserPromptTemplate, userKeyValues),
            //    SystemPrompt = DynamicPromptTemplateProcessor.GetPromptStringFromTemplate(dynamicPrompt.SystemPromptTemplate, systemKeyValues),
            //    Temperature = dynamicPrompt.Temperature,
            //    Seed = dynamicPrompt.Seed
            //};
        }
    }
}