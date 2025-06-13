using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PromptEngineering.Hubs
{
    public interface ITaskUpdateService
    {
        Task SendMessage1(string user, string message);
        Task SendParamsTesterStatus(int instanceId, int resultId, bool isComplete, bool isRunning, int durationMs);
        Task SendParamsTesterDone(int instanceId, string friendlyName);

        //

        Task SendConversionInstanceStatus(int instanceId, int status);
        Task SendConversionInstanceDoneStatus(int instanceId, int status, int resultFileCollectionId);
        Task SendConversionInstanceLogsAdded(int instanceId);
    }
}