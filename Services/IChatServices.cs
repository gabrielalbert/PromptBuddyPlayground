
using PromptEngineering.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PromptEngineering.Services
{
    public interface IChatServices
    {
        Task<ChatMessage> GetChatMessage(ChatInput input);

        Task<IEnumerable<ChatMessage>> GetAllChatMessages(string userName, string startDate = "", string endDate = "");

        Task<DashboardInfoPromptEnggTypes> GetNumbersOfPromptEnggTypes();

        Task<IEnumerable<string>> GetAutoCompleteSuggestions(string aiModel, string query);

        Task UpdateFeedback(int chatId, string feedback);       
    }
}