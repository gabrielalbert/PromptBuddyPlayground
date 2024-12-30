using PromptEngineering.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace PromptEngineering.Repository
{
    /// <summary>
    /// Interface for chat-related database operations.
    /// </summary>
    public interface IChatRepository
    {
        Task<IEnumerable<ChatMessage>> GetAllChatMessagesAsync(string userName, string startDate = "", string endDate = "");

        Task<DashboardInfoPromptEnggTypes> GetNumbersOfPromptEnggTypesAsync();

        Task<IEnumerable<string>> GetAutoCompleteSuggestions(string aiModel, string query);

        Task UpdateFeedback(int chatId, string feedback);

        int AddChats(Chats chats);

        Task<string> GetChatMessage(ChatInput input);

        (string endpointUrl, string apiToken) GetAiEndpointUrl(string llm, string aiModel);

    }
}