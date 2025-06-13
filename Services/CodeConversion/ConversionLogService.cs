using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PromptEngineering.Data;
using PromptEngineering.Hubs;
using PromptEngineering.Models.CodeConversion;

namespace PromptEngineering.Services.CodeConversion
{
    public class ConversionLogService : IConversionLogService
    {
        private IApplicationDBContext ApplicationDBContext;
        private ITaskUpdateService TaskUpdateService;

        public ConversionLogService(IApplicationDBContext applicationDBContext, ITaskUpdateService taskUpdateService)
        {
            this.ApplicationDBContext = applicationDBContext;
            this.TaskUpdateService = taskUpdateService;
        }

        public async Task Log(string titleText, string detailedText, ConversionInstanceLogType logType, int conversionInstanceId)
        {
            await ApplicationDBContext.AddConversionInstanceLog(
                conversionInstanceId,
                titleText,
                detailedText, ConversionInstanceLogType.PROMPT_SENDING
            );
            await TaskUpdateService.SendConversionInstanceLogsAdded(conversionInstanceId);
        }
    }
}