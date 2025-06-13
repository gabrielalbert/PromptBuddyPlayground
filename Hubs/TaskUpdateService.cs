using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace PromptEngineering.Hubs
{
    public class TaskUpdateService : ITaskUpdateService
    {
        private IHubContext<TaskUpdateHub> TaskUpdateHubContext { get; set; }
        public TaskUpdateService(IHubContext<TaskUpdateHub> taskUpdateHubContext) {
            TaskUpdateHubContext = taskUpdateHubContext;
        }

        public async Task SendMessage1(string user, string message)
        {
            await TaskUpdateHubContext.Clients.All.SendAsync("SendMessage1", user, message);
        }

        public async Task SendParamsTesterStatus(int instanceId, int resultId, bool isComplete, bool isRunning, int durationMs) 
        {
            await TaskUpdateHubContext.Clients.All.SendAsync("ParamsTesterStatusMessage", instanceId, resultId, isComplete, isRunning, durationMs);
        }

        public async Task SendParamsTesterDone(int instanceId, string friendlyName) {
            await TaskUpdateHubContext.Clients.All.SendAsync("ParamsTesterStatusDone", instanceId, friendlyName);
        }

        public async Task SendConversionInstanceStatus(int instanceId, int status) {
            await TaskUpdateHubContext.Clients.All.SendAsync("ConversionInstanceStatus", instanceId, status);
        }

        public async Task SendConversionInstanceDoneStatus(int instanceId, int status, int resultFileCollectionId) {
            await TaskUpdateHubContext.Clients.All.SendAsync("ConversionInstanceDoneStatus", instanceId, status, resultFileCollectionId);
        }
        
        public async Task SendConversionInstanceLogsAdded(int instanceId) {
            await TaskUpdateHubContext.Clients.All.SendAsync("ConversionInstanceLogsAdded", instanceId);
        }
    }
}