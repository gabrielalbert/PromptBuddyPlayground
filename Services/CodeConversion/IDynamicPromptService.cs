using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PromptEngineering.Models.DynamicPrompt;

namespace PromptEngineering.Services.CodeConversion
{
    public interface IDynamicPromptService
    {
        Task<LLMPrompt> GetDynamicPrompt(string promptKey, Dictionary<string, string> userKeyValues, Dictionary<string, string> systemKeyValues);
    }
}