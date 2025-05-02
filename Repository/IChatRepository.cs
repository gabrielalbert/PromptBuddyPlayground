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
        Task<IEnumerable<ChatMessage>> GetAllChatMessagesAsync(string userName, string messageId, string startDate = "", string endDate = "");

        Task<DashboardInfoPromptEnggTypes> GetNumbersOfPromptEnggTypesAsync();

        Task<IEnumerable<string>> GetAutoCompleteSuggestions(string aiModel, string query);

        Task UpdateFeedback(int chatId, string feedback);

        int AddChats(Chats chats);

        Task<string> GetChatMessage(ChatInput input);

        (string endpointUrl, string apiToken) GetAiEndpointUrl(string llm, string aiModel);
        List<ConversationDetailsModel> GetConversations(int conversationId);
        int AddGroupName(int groupId, string groupChatName);
        string GetUserByID(int userId);
        Task<IEnumerable<ChatGroups>> GetRecentChatsAsync(int userId);
        Task<IEnumerable<ChatMessage>> GetAllChatMessagesByGroupIDAsync(int groupID);

    }
}