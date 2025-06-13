using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PromptEngineering.Models.CodeConversion
{
    public enum ConversionInstanceLogType {
        SUCCESS,
        INFO,
        WARNING,
        PROMPT_SENDING,
        PROMPT_SUCCESS,
        PROMPT_FINAL_SUCCESS
    }
}