using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PromptEngineering.Models;
using PromptEngineering.Repository;
using System;
using Microsoft.Extensions.AI;
using OpenAI.Chat;
using System.Collections.Generic;
using Microsoft.VisualBasic;
using System.Threading.Tasks;


namespace PromptEngineering.Services
{

    public class OllamaServices : IOllamaServices
    {
        private readonly IChatRepository _chatRepository;
        private readonly ILogger<OllamaServices> _logger;        
        

        public OllamaServices(ILogger<OllamaServices> logger, IChatRepository chatRepository)
        {
            _logger = logger;            
            _chatRepository = chatRepository;                        
        }

        private List<Microsoft.Extensions.AI.ChatMessage> buildConversations(int conversationId = 0)
        {
            List<Microsoft.Extensions.AI.ChatMessage> messages = new List<Microsoft.Extensions.AI.ChatMessage>();

            if (conversationId > 0)
            {
                var conversations = _chatRepository.GetConversations(conversationId);
                foreach (var conversation in conversations)
                {
                    var userRequest = new Microsoft.Extensions.AI.ChatMessage(ChatRole.User, conversation.PromptMessgae);
                    messages.Add(userRequest);
                    var aiResponse = new Microsoft.Extensions.AI.ChatMessage(ChatRole.Assistant, conversation.PromptMessgae);
                    messages.Add(aiResponse);
                }
            }

            return messages;

        }

        public async Task<Microsoft.Extensions.AI.ChatResponse> GetChatMessageFromLocalPhiAi(string command, string endpointUrl, int conversationId=0,string model= "llama3.2:3b")
        {  

            IChatClient client = new OllamaChatClient(new Uri(endpointUrl), model);            
            
            try
            {
                var messages = buildConversations(conversationId);

                messages.Add(new Microsoft.Extensions.AI.ChatMessage(ChatRole.User, command));
                var ollama = client.GetRequiredService<Microsoft.Extensions.AI.IChatClient>();
                return await ollama.GetResponseAsync(messages);   

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw ex;

            }            
        }
    }
}
