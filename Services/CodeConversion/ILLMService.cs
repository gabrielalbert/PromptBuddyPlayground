using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PromptEngineering.Models.DynamicPrompt;

namespace PromptEngineering.Services.CodeConversion
{
    public interface ILLMService
    {
        Task<PromptTestResponse> GetResponseDirect(LLMPrompt promptInput);
    }
}