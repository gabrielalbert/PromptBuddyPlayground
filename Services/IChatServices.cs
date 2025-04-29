
using PromptEngineering.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PromptEngineering.Services
{
    public interface IChatServices
    {
        Task<ChatMessage> GetChatMessage(ChatInput input);

        Task<IEnumerable<ChatMessage>> GetAllChatMessages(string userName, string messageId, string startDate = "", string endDate = "");

        Task<DashboardInfoPromptEnggTypes> GetNumbersOfPromptEnggTypes();

        Task<IEnumerable<string>> GetAutoCompleteSuggestions(string aiModel, string query);

        Task UpdateFeedback(int chatId, string feedback);        
        Task<ChatMessage> GetNewChatMessage(AiAssistantInput newInput);
        Task<IEnumerable<ChatGroups>> GetRecentChatsAsync();
        Task<IEnumerable<ChatMessage>> GetAllChatMessagesByGroupID(int groupId, string messageId, string startDate = "", string endDate = "");

    }
}