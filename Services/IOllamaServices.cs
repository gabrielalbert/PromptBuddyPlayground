
using PromptEngineering.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PromptEngineering.Services
{
    public interface IOllamaServices
    {
        Task<Microsoft.Extensions.AI.ChatResponse> GetChatMessageFromLocalPhiAi(string command, string endpointUrl, int conversationId = 0, string model = "llama3.2:3b");
        
    }
}