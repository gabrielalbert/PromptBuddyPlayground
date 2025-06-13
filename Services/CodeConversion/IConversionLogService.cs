using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PromptEngineering.Models.CodeConversion;

namespace PromptEngineering.Services.CodeConversion
{
    public interface IConversionLogService
    {
        Task Log(string titleText, string detailedText, ConversionInstanceLogType logType, int conversionInstanceId);
    }
}