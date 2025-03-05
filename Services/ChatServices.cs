using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenAI;
using OpenAI.Chat;
using PromptEngineering.Models;
using PromptEngineering.Repository;
using PromptEngineering.Utils;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using ChatMessage = PromptEngineering.Models.ChatMessage;

namespace PromptEngineering.Services
{
    
    public class ChatServices : IChatServices
    {
        private readonly IChatRepository _chatRepository;
        private readonly ILogger<ChatServices> _logger;        
        private readonly HttpClient _httpClient;
        private readonly IMapper _mapper;
        private readonly MySettings _settings;
        private readonly ICopilotCLIServices _copilotCLIServices;
        private readonly IFilesServices _filesServices;



        public ChatServices(ILogger<ChatServices> logger, IFilesServices filesServices, ICopilotCLIServices copilotCLIServices, IMapper mapper, IChatRepository chatRepository, IHttpClientFactory httpClientFactory, IOptions<MySettings> settings)
        {
            _logger = logger;
            _filesServices = filesServices;
            _copilotCLIServices = copilotCLIServices;
            _mapper = mapper;
            _chatRepository = chatRepository;            
            _httpClient = httpClientFactory.CreateClient();
        }
        

        public async Task<IEnumerable<ChatMessage>> GetAllChatMessages(string userName, string messageId, string startDate = "", string endDate="")
        {
            return await _chatRepository.GetAllChatMessagesAsync(userName,messageId, startDate,endDate);
        }

        public async Task<DashboardInfoPromptEnggTypes> GetNumbersOfPromptEnggTypes()
        {
            return await _chatRepository.GetNumbersOfPromptEnggTypesAsync();
        }

        public async Task<IEnumerable<string>> GetAutoCompleteSuggestions(string aiModel, string query)
        {
            return await _chatRepository.GetAutoCompleteSuggestions(aiModel, query);
        }

        public async Task UpdateFeedback(int chatId, string feedback)
        {
            await _chatRepository.UpdateFeedback(chatId, feedback);
        }
                
        public async Task<ChatMessage> GetChatMessage(ChatInput input)
        {
            var replyMessage = new ChatMessage();            
            int chatId = 0;
            try
            {

                string result = await _chatRepository.GetChatMessage(input);

                var referenceFileName = (input.FileReference ? input.Reference : string.Empty);
                if(input.FileReference)
                {
                    input.Reference = await _filesServices.ReadFileContent(input.Reference);
                }
                

                Chats chats = _mapper.Map<Chats>(input);
                chats.RequestedTime = DateTime.Now;
                chats.FileName = (input.FileReference ? referenceFileName : string.Empty);
                chats.MessageKey = string.Concat(input.LLM.ToLower().Trim(),"|", input.Model.ToLower().Trim());
                if (!string.IsNullOrEmpty(result))
                {
                    chats.RespondedTime = DateTime.Now;
                    chats.Attempt = 0;
                    chats.InputTokens = 0;
                    chats.OutputTokens = 0;
                    chats.TotalTokens = 0;
                    chats.AI=false;
                    chats.PromptEnggType = "AvailableResult";
                    chatId = _chatRepository.AddChats(chats);
                    replyMessage.ChatId = chatId;
                    replyMessage.MessageText = result.ReplaceEscapeChars();
                    replyMessage.MessageSender = MessageSender.Bot;
                    replyMessage.MessageDate = DateTime.Now;
                    replyMessage.MessageId = chatId;
                    replyMessage.PromptEnggType = "AvailableResult";
                    replyMessage.Feedback = string.Empty;
                }
                else
                {
                    if (chats.LLM == "copilot" && chats.Model == "cli")
                    {
                        chats = await _copilotCLIServices.ExecuteCopilotCommand(chats, false);
                    }
                    else 
                    {
                        chats = await Prompting(chats);
                        if (chats.ConversationId > 0)
                        {
                            chats.PromptEnggType = PromptEnggType.ChainofThought;
                        }
                        else
                        {
                            chats.PromptEnggType = PromptEnggType.ZeroShot;
                        }

                    }
                    replyMessage.RawMessageText = chats.Result;
                    replyMessage.MessageText = chats.Result.ReplaceEscapeChars();
                    replyMessage.MessageSender = "bot";
                    replyMessage.MessageDate = chats.RespondedTime;
                    replyMessage.PromptEnggType = chats.PromptEnggType;
                    chatId = _chatRepository.AddChats(chats);
                    _logger.LogInformation($"Command executed successfully with output: {chats.Result}");
                    replyMessage.ChatId = chatId;
                    replyMessage.MessageId = chatId;

                    _logger.LogInformation($"GetPromptMessage with output: {result}");
                }

            }
            catch (Exception ex)
            {
                replyMessage.ChatId = -1;
                replyMessage.RawMessageText = ex.Message;
                replyMessage.MessageText = "An error occurred";
                replyMessage.MessageSender = MessageSender.Bot;
                replyMessage.Feedback = string.Empty;
                replyMessage.MessageDate = DateTime.Now;
                replyMessage.MessageId = -1;
                replyMessage.PromptEnggType = "Exception";
                _logger.LogError(ex.Message);
            }
            return replyMessage;
        }
        private async Task<string> RetrieveContextFromRepo(string reponame, string prompt)
        {
            HttpClient client = new HttpClient();
            string apiUrl = _settings.RepoUrl;
            try
            {
                var requestData = new
                {
                    collection_name = reponame,
                    query_text = prompt,
                    n_results = 5
                };
                string jsonData = JsonSerializer.Serialize(requestData);
                StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(apiUrl, content);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                _logger.LogInformation($"Retrieved Context: {responseBody}");
                var docs = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, object>>>(responseBody)["results"]["documents"];
                string docsString = string.Join("\n", (JsonSerializer.Deserialize<string[][]>(docs.ToString()))[0]);
                return prompt + "\n\nConsider the following context:\n" + docsString;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        private async Task<Chats> Prompting(Chats message)
        {

            try
            {
                string promptCommand = GetPromptCommand(message.Phase, message.Prompt, message.Reference, message.Language, message.PhaseOptional);
                if (!(string.IsNullOrEmpty(message.RepoName) || (message.RepoName.ToLower() == "public")))
                {
                    string retrievedContext = await RetrieveContextFromRepo(message.RepoName, promptCommand);
                    promptCommand = retrievedContext;
                }

                _logger.LogInformation($"Prompting Prompt Command: {promptCommand}");
                RetriveFromAi(ref message, promptCommand);
                message.PromptMessage = promptCommand;                
                _logger.LogInformation($"Prompting output string {message.Result}");

            }
            catch (Exception ex)
            {
                _logger.LogInformation($"ChainPrompt failed: {ex.Message}");
            }

            return message;
        }

        private async Task<Chats> ZeroShotPrompt(Chats message)
        {

            try
            {
                string promptCommand = GetPromptCommand(message.Phase, message.Prompt, message.Reference, message.Language, message.PhaseOptional);

                _logger.LogInformation($"ZeroShotPrompt Prompt Command: {promptCommand}");
                RetriveFromAi(ref message,promptCommand);
                message.PromptMessage = promptCommand;
                message.PromptEnggType = PromptEnggType.ZeroShot;
                _logger.LogInformation($"ZeroShotPrompt output string {message.Result}");                

            }
            catch (Exception ex)
            {
                _logger.LogInformation($"ZeroShotPrompt failed: {ex.Message}");
            }

            return message;
        }

        private string GetPromptCommand(string phase, string command, string referenceCode, string language, string phaseOptional, bool repharse = false)
        {
            _logger.LogInformation($"GetPromptCommand method started at {DateTime.Now}");
            _logger.LogInformation($"GetPromptCommand method Input {phase} {command} {referenceCode}");
            try
            {
                string PromptCommand = string.Empty;
                //if (language.Equals("General", StringComparison.OrdinalIgnoreCase) && phase.Equals(CLIPhase.OTHER, StringComparison.OrdinalIgnoreCase))
                //{
                //    GeminiCommand = $" {(repharse ? CLI.REPHRASE : string.Empty)} {command} {(string.IsNullOrEmpty(referenceCode) ? string.Empty : "'" + referenceCode.ReplaceNewLineChars() + "'")}";
                //}
                //else if (language.Equals("General", StringComparison.OrdinalIgnoreCase) && phase.Equals(CLIPhase.CODE_GENERATION, StringComparison.OrdinalIgnoreCase))
                //{
                //    GeminiCommand = $" {(repharse ? CLI.REPHRASE : string.Empty)} {CLI.CODE} \"{command} {(string.IsNullOrEmpty(referenceCode) ? string.Empty : "'" + referenceCode.ReplaceNewLineChars() + "'")}\"";
                //}
                //else if (language.Equals("General", StringComparison.OrdinalIgnoreCase) && phase.Equals(CLIPhase.UNIT_TEST, StringComparison.OrdinalIgnoreCase))
                //{
                //    GeminiCommand = $" {(repharse ? CLI.REPHRASE : string.Empty)} {CLI.UNIT_TEST} \"{command} {(string.IsNullOrEmpty(referenceCode) ? string.Empty : "'" + referenceCode.ReplaceNewLineChars() + "'")}\"";
                //}
                //else if (language.Equals("General", StringComparison.OrdinalIgnoreCase) && phase.Equals(CLIPhase.BUG_FIX, StringComparison.OrdinalIgnoreCase))
                //{
                //    GeminiCommand = $" {(repharse ? CLI.REPHRASE : string.Empty)} {CLI.BUG_FIX} \"{command} {(string.IsNullOrEmpty(referenceCode) ? string.Empty : "'" + referenceCode.ReplaceNewLineChars() + "'")}\"";
                //}
                //else if (language.Equals("General", StringComparison.OrdinalIgnoreCase) && phase.Equals(CLIPhase.XMLDOCS, StringComparison.OrdinalIgnoreCase))
                //{
                //    GeminiCommand = $" {(repharse ? CLI.REPHRASE : string.Empty)} {CLI.XML_DOCS} \"{command} {(string.IsNullOrEmpty(referenceCode) ? string.Empty : "'" + referenceCode.ReplaceNewLineChars() + "'")}\"";
                //}
                //else if (language.Equals("General", StringComparison.OrdinalIgnoreCase) && phase.Equals(CLIPhase.DOCS, StringComparison.OrdinalIgnoreCase))
                //{
                //    GeminiCommand = $" {(repharse ? CLI.REPHRASE : string.Empty)} {CLI.DOCS} \"{command} {(string.IsNullOrEmpty(referenceCode) ? string.Empty : "'" + referenceCode.ReplaceNewLineChars() + "'")}\"";
                //}
                //else if (language.Equals("General", StringComparison.OrdinalIgnoreCase) && phase.Equals(CLIPhase.SECURITY_FIX, StringComparison.OrdinalIgnoreCase))
                //{
                //    GeminiCommand = $" {(repharse ? CLI.REPHRASE : string.Empty)} {CLI.SECURITY_FIX} \"{command} {(string.IsNullOrEmpty(referenceCode) ? string.Empty : "'" + referenceCode.ReplaceNewLineChars() + "'")}\"";
                //}                

                if (phase.Equals(CLIPhase.CONVERT, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(language) && !string.IsNullOrEmpty(phaseOptional))
                {   
                    string addCode = (string.IsNullOrEmpty(referenceCode) ? string.Empty : string.Format(Configurations.ADD_REFERENCE_CODE, referenceCode.ReplaceNewLineChars()));
                    PromptCommand = $" {CLIPhase.CONVERT} {phaseOptional} into {language}, {command} {addCode} ";
                    _logger.LogInformation($"CLI command with lang added {PromptCommand}");
                }
                else if (!language.Equals("General", StringComparison.OrdinalIgnoreCase) && phase.Equals(CLIPhase.CODE, StringComparison.OrdinalIgnoreCase))
                {
                    string addLang = (command.Contains(language, StringComparison.OrdinalIgnoreCase) ? string.Empty : string.Format(Configurations.ADD_PROG_LANG, language));
                    string addCommand = (string.IsNullOrEmpty(command) ? string.Empty : string.Format(Configurations.ADD_GENERATE_CODE, command));
                    string addCode = (string.IsNullOrEmpty(referenceCode) ? string.Empty : string.Format(Configurations.ADD_REFERENCE_CODE, referenceCode.ReplaceNewLineChars()));
                    PromptCommand = $" {CLIPhase.CODE} \"{addLang} {addCommand} {addCode}\"";
                    _logger.LogInformation($"CLI command with lang added {PromptCommand}");
                }
                else if (!language.Equals("General", StringComparison.OrdinalIgnoreCase) && phase.Equals(CLIPhase.UNIT_TEST, StringComparison.OrdinalIgnoreCase))
                {
                    string addLang = (command.Contains(language, StringComparison.OrdinalIgnoreCase) ? string.Empty : string.Format(Configurations.ADD_PROG_LANG, language));
                    string addCommand = (string.IsNullOrEmpty(command) ? Configurations.ADD_DEFAULT_UNIT_TEST : string.Format(Configurations.ADD_GENERATE_CODE, command));
                    string addCode = (string.IsNullOrEmpty(referenceCode) ? string.Empty : string.Format(Configurations.ADD_REFERENCE_CODE, referenceCode.ReplaceNewLineChars()));
                    PromptCommand = $" {CLIPhase.UNIT_TEST} \"{addLang} {addCommand} {addCode}\"";
                    _logger.LogInformation($"CLI command with lang added {PromptCommand}");
                }
                else if (!language.Equals("General", StringComparison.OrdinalIgnoreCase) && phase.Equals(CLIPhase.BUG_FIX, StringComparison.OrdinalIgnoreCase))
                {
                    string addLang = (command.Contains(language, StringComparison.OrdinalIgnoreCase) ? string.Empty : string.Format(Configurations.ADD_PROG_LANG, language));
                    string addCommand = (string.IsNullOrEmpty(command) ? Configurations.ADD_DEFAULT_UNIT_TEST : string.Format(Configurations.ADD_GENERATE_CODE, command));
                    string addCode = (string.IsNullOrEmpty(referenceCode) ? string.Empty : string.Format(Configurations.ADD_REFERENCE_CODE, referenceCode.ReplaceNewLineChars()));
                    PromptCommand = $" {CLIPhase.BUG_FIX} \"{addLang} {addCommand} {addCode} \"";
                    _logger.LogInformation($"CLI command with lang added {PromptCommand}");
                }
                else if (!language.Equals("General", StringComparison.OrdinalIgnoreCase) && phase.Equals(CLIPhase.DOCS, StringComparison.OrdinalIgnoreCase))
                {
                    string addLang = (command.Contains(language, StringComparison.OrdinalIgnoreCase) ? string.Empty : string.Format(Configurations.ADD_PROG_LANG, language));
                    string addCommand = (string.IsNullOrEmpty(command) ? Configurations.ADD_DEFAULT_UNIT_TEST : string.Format(Configurations.ADD_GENERATE_CODE, command));
                    string addCode = (string.IsNullOrEmpty(referenceCode) ? string.Empty : string.Format(Configurations.ADD_REFERENCE_CODE, referenceCode.ReplaceNewLineChars()));
                    PromptCommand = $" {CLIPhase.DOCS} \"{addLang} {addCommand} {addCode} \"";
                    _logger.LogInformation($"CLI command with lang added {PromptCommand}");
                }
                else if (!language.Equals("General", StringComparison.OrdinalIgnoreCase) && phase.Equals(CLIPhase.XMLDOCS, StringComparison.OrdinalIgnoreCase))
                {
                    string addLang = (command.Contains(language, StringComparison.OrdinalIgnoreCase) ? string.Empty : string.Format(Configurations.ADD_PROG_LANG, language));
                    string addCommand = (string.IsNullOrEmpty(command) ? Configurations.ADD_DEFAULT_UNIT_TEST : string.Format(Configurations.ADD_GENERATE_CODE, command));
                    string addCode = (string.IsNullOrEmpty(referenceCode) ? string.Empty : string.Format(Configurations.ADD_REFERENCE_CODE, referenceCode.ReplaceNewLineChars()));
                    PromptCommand = $" {CLIPhase.XMLDOCS} \"{addLang} {addCommand} {addCode} \"";
                    _logger.LogInformation($"CLI command with lang added {PromptCommand}");
                }
                else if (language.Equals("General", StringComparison.OrdinalIgnoreCase) && phase.Equals(CLIPhase.OTHER, StringComparison.OrdinalIgnoreCase))
                {
                    PromptCommand = command;
                    _logger.LogInformation(command);
                }

            _logger.LogInformation($"CLI command framed {PromptCommand}");
                return PromptCommand;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw ex;
            }

        }
        private List<OpenAI.Chat.ChatMessage> buildConversations(int conversationId = 0)
        {
            List<OpenAI.Chat.ChatMessage> messages = new List<OpenAI.Chat.ChatMessage>();

            if(conversationId > 0)
            {
                var conversations = _chatRepository.GetConversations(conversationId);
                foreach(var conversation in conversations)
                {
                    var userRequest = new UserChatMessage(conversation.PromptMessgae);
                    messages.Add(userRequest);
                    var aiResponse = new AssistantChatMessage(conversation.AiResponse);
                    messages.Add(aiResponse);

                }
            }

            return messages;

        }


        private void RetriveFromAi(ref Chats message,string command)
        {
            var result = _chatRepository.GetAiEndpointUrl(message.LLM,message.Model);           

            var credential = new System.ClientModel.ApiKeyCredential(result.apiToken);

            var openAIOptions = new OpenAIClientOptions()
            {
                Endpoint = new System.Uri(result.endpointUrl)
            };

            var client = new ChatClient(message.Model, credential, openAIOptions);
            
            try
            {
                var messages = buildConversations(message.ConversationId);

                messages.Add(new UserChatMessage(command));

                var requestOptions = new ChatCompletionOptions()
                {
                };

                var response = client.CompleteChat(messages, requestOptions);                

                message.Result = response.Value.Content[0].Text;
                message.RespondedTime = DateTime.Now;
                message.Status = "Completed";
                
                message.Success = true;
                message.AI = true;
                message.Attempt = 1;
                message.InputTokens = response.Value.Usage.InputTokenCount;
                message.OutputTokens = response.Value.Usage.OutputTokenCount;
                message.TotalTokens = message.InputTokens + message.OutputTokens;
                

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                message.Status = "Exception";
                message.Result=ex.Message;
                message.Success = false;
                message.AI = true;
                message.Attempt = 1;
                message.InputTokens =0;
                message.OutputTokens = 0;
                message.TotalTokens = 0;

            }
            
        }


    }
}
