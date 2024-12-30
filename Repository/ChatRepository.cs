using PromptEngineering.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PromptEngineering.Utils;
using Microsoft.AspNetCore.DataProtection.KeyManagement;

namespace PromptEngineering.Repository
{
    /// <summary>
    /// Provides methods to interact with the PostgreSQL database for chat-related operations.
    /// </summary>
    public class ChatRepository : IChatRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<ChatRepository> _logger;

        public ChatRepository(ILogger<ChatRepository> logger, IConfiguration configuration)
        {
            _logger = logger;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        #region GetAllChatMessages
        public async Task<IEnumerable<ChatMessage>> GetAllChatMessagesAsync(string userName, string startDate = "",string endDate="")
        {
            _logger.LogInformation($"GetAllChatMessagesAsync method Input {userName} {startDate} {endDate}");
            var chatMessages = new List<ChatMessage>();
            try
            {
                var sql = @"select llm,ai_model,prog_lang, phase,phase_optional,prompt,reference,result,status,attempt,success,requested_time,responded_time,user_name,prompt_engg_type,chat_id,feedback from  chats where  user_name=@user_name";
                if ((!string.IsNullOrEmpty(startDate) && string.IsNullOrEmpty(endDate))||(!string.IsNullOrEmpty(startDate) && string.Equals(startDate,endDate,StringComparison.OrdinalIgnoreCase)))
                {
                    sql += " and cast(requested_time as date)=cast(@requested_time as date)";
                }
                else if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
                {
                    sql += " AND cast(requested_time as date) BETWEEN cast(@start_date as date) AND cast(@end_date  as date)";

                }
                sql += " order by chat_id asc;";
                _logger.LogInformation($"GetAllChatMessagesAsync Query {sql} ");

                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new NpgsqlCommand(sql, connection))
                    {                        
                        command.Parameters.AddWithValue("@user_name", userName);
                        
                        if ((!string.IsNullOrEmpty(startDate) && string.IsNullOrEmpty(endDate)) || ( !string.IsNullOrEmpty(startDate)&& string.Equals(startDate, endDate, StringComparison.OrdinalIgnoreCase)))
                        {
                            command.Parameters.AddWithValue("@requested_time", DateTime.Parse(startDate).ToString("yyyy-MM-dd"));
                        }
                        else if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
                        {
                            command.Parameters.AddWithValue("@start_date", DateTime.Parse(startDate).ToString("yyyy-MM-dd"));
                            command.Parameters.AddWithValue("@end_date", DateTime.Parse(endDate).ToString("yyyy-MM-dd"));
                        }

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            int index = 1;
                            while (await reader.ReadAsync())
                            {
                                string feedback=(reader.IsDBNull(16)?"":reader.GetString(16));
                                var aiSendMessage = new ChatMessage();
                                string sendMessage = @$"AI Model: {reader.GetString(0)}-{reader.GetString(1)}, Language: {reader.GetString(2)}, Phase: {reader.GetString(3)}, Prompt:{reader.GetString(5)}, Reference: {reader.GetString(6)}";
                                aiSendMessage.MessageText = sendMessage.ReplaceEscapeChars();
                                aiSendMessage.MessageSender = MessageSender.User;
                                aiSendMessage.MessageDate = reader.GetDateTime(11);
                                aiSendMessage.PromptEnggType = reader.GetString(14);
                                aiSendMessage.MessageId = index++;
                                aiSendMessage.ChatId = reader.GetInt32(15);
                                aiSendMessage.Feedback = (string.IsNullOrEmpty(feedback)?"":feedback);
                                chatMessages.Add(aiSendMessage);
                                _logger.LogInformation($"Sender: {aiSendMessage.MessageSender} Output: {aiSendMessage.MessageText}");
                                var aiReplyMessage = new ChatMessage();
                                string replyMessage = reader.GetString(7);
                                aiReplyMessage.MessageText = replyMessage.ReplaceEscapeChars();
                                aiReplyMessage.MessageSender = MessageSender.Bot;
                                aiReplyMessage.MessageDate = reader.GetDateTime(12);
                                aiReplyMessage.PromptEnggType = reader.GetString(14);
                                aiReplyMessage.MessageId = index++;
                                aiReplyMessage.ChatId = reader.GetInt32(15);
                                aiReplyMessage.Feedback =(string.IsNullOrEmpty(feedback)?"":feedback);
                                chatMessages.Add(aiReplyMessage);
                                _logger.LogInformation($"Sender: {aiReplyMessage.MessageSender} Output: {aiReplyMessage.MessageText}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
            }
            return chatMessages;
        }
        #endregion        

        #region GetNumbersOfPromptEnggTypes
        public async Task<DashboardInfoPromptEnggTypes> GetNumbersOfPromptEnggTypesAsync()
        {
            _logger.LogInformation($"GetNumbersOfPromptEnggTypesAsync method");
            var dashboardInfo = new DashboardInfoPromptEnggTypes();
            try
            {
                var sql = @"select zeroShots,oneShots,iterativeShots FROM public.get_dashboard_data();";

                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                dashboardInfo.ZeroShot = Convert.ToString(reader.GetFieldValue<long>(0));
                                dashboardInfo.OneShot = Convert.ToString(reader.GetFieldValue<long>(1));
                                dashboardInfo.IterativeShot = Convert.ToString(reader.GetFieldValue<long>(2));
                                _logger.LogInformation($"ZeroShot: {dashboardInfo.ZeroShot} OneShot: {dashboardInfo.OneShot} IterativeShot: {dashboardInfo.IterativeShot} ");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
            }
            return dashboardInfo;
        }
        #endregion

        #region GetAutoCompleteSuggestions

        public async Task<IEnumerable<string>> GetAutoCompleteSuggestions(string aiModel, string query)
        {
            _logger.LogInformation($"GetAutoCompleteSuggestions method Input {aiModel}");
            var autoCompleteSuggestions = new List<string>();
            try
            {
                var sql = @$"select prompt from chats where ai_model=(@ai_model) AND prompt like '%{query}%' group by prompt ;";

                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@ai_model", aiModel);
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                autoCompleteSuggestions.Add(reader.GetString(0));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
            }
            return autoCompleteSuggestions;
        }

        #endregion

        #region UpdateFeedback
        public async Task UpdateFeedback(int chatId,string feedback)
        {
            _logger.LogInformation($"AcceptFeedback method Input {chatId} {feedback}");            
            try
            {
                var sql = @$"update chats set feedback=@feedback where chat_id=(@chatId);";

                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@feedback", feedback);
                        command.Parameters.AddWithValue("@chatId", chatId);
                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
            }
        }
        #endregion
        
        #region AddChats

        public int AddChats(Chats chats)
        {
            _logger.LogInformation($"AddChatsAsync method");

            var sql = @"INSERT INTO chats (message_id,conversation_id,llm,ai_model, prog_lang, phase,phase_optional,prompt,reference,result,status,attempt,success,requested_time,responded_time,input_tokens," +
                "output_tokens,total_tokens,is_ai,user_name,role_name,prompt_engg_type) VALUES(@message_id,@conversation_id,@llm,@ai_model, @prog_lang, @phase,@phase_optional,@prompt,@reference,@result," +
                "@status,@attempt,@success,@requested_time,@responded_time,@input_tokens,@output_tokens,@total_tokens,@is_ai,@user_name,@role_name,@prompt_engg_type) returning chat_id;";

            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@message_id", chats.MessageId);
                        command.Parameters.AddWithValue("@conversation_id", chats.ConversationId);
                        command.Parameters.AddWithValue("@llm", chats.LLM);
                        command.Parameters.AddWithValue("@ai_model", chats.Model);
                        command.Parameters.AddWithValue("@prog_lang", chats.Language);
                        command.Parameters.AddWithValue("@phase", chats.Phase);
                        command.Parameters.AddWithValue("@phase_optional", (string.IsNullOrEmpty(chats.PhaseOptional) ? "" : chats.PhaseOptional));
                        command.Parameters.AddWithValue("@prompt", chats.Prompt);
                        command.Parameters.AddWithValue("@reference", chats.Reference);
                        command.Parameters.AddWithValue("@result", (string.IsNullOrEmpty(chats.Result) ? "" : chats.Result));
                        command.Parameters.AddWithValue("@status", chats.Status);
                        command.Parameters.AddWithValue("@attempt", chats.Attempt);
                        command.Parameters.AddWithValue("@success", chats.Success);
                        command.Parameters.AddWithValue("@requested_time", chats.RequestedTime);
                        command.Parameters.AddWithValue("@responded_time", chats.RespondedTime);
                        command.Parameters.AddWithValue("@input_tokens", chats.InputTokens);
                        command.Parameters.AddWithValue("@output_tokens", chats.OutputTokens);
                        command.Parameters.AddWithValue("@total_tokens", chats.TotalTokens);
                        command.Parameters.AddWithValue("@is_ai", chats.AI);
                        command.Parameters.AddWithValue("@user_name", chats.SelectedUser);
                        command.Parameters.AddWithValue("@role_name", chats.SelectedRole);
                        command.Parameters.AddWithValue("@prompt_engg_type", chats.PromptEnggType);                        
                        int chatId = (int)command.ExecuteScalar();
                        _logger.LogInformation("The row has been inserted successfully.");
                        return chatId;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
            }
            return -1;
        }

        #endregion

        #region GetChatMessage
        public async Task<string> GetChatMessage(ChatInput input)
        {
            _logger.LogInformation($"GetChatMessage Chat Repo method Input {input.LLM} {input.Model} {input.Language} {input.Phase} {input.PhaseOptional} {input.Prompt} {input.Reference} {input.CurrentUser} {input.SelectedRole} ");
            var result = string.Empty;
            try
            {
                var sql = @$"select result from chats where feedback='helpful' and llm=@llm and ai_model=@ai_model and prog_lang=@prog_lang and phase=@phase and isnull(phase_optional,'')=isnull(@phase_optional,'') and prompt=@prompt and reference=@reference and user_name=@user_name order by chat_id desc limit 1;";

                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@llm", input.LLM);
                        command.Parameters.AddWithValue("@ai_model", input.Model);
                        command.Parameters.AddWithValue("@prog_lang", input.Language);
                        command.Parameters.AddWithValue("@phase", input.Phase);
                        command.Parameters.AddWithValue("@phase_optional", input.PhaseOptional);
                        command.Parameters.AddWithValue("@prompt", input.Prompt);
                        command.Parameters.AddWithValue("@reference", input.Reference);
                        command.Parameters.AddWithValue("@user_name", input.CurrentUser);
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                result = reader.GetString(0);
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
            }

            return result;
        }
        #endregion

        #region GetAiEndpointUrl
        public (string endpointUrl, string apiToken) GetAiEndpointUrl(string llm, string aiModel)
        {
            _logger.LogInformation($"GetAiEndpointUrl method Input {llm} {aiModel}");
            var endpointUrl = string.Empty;
            var apiToken = string.Empty;
            
            string llm_key = llm.ToLower().Trim() + aiModel.ToLower().Trim();
            try
            {
                var sql = @$"select endpoint,api_token from ai_config where llm_key=@llm_key;";

                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@llm_key", llm_key);
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                endpointUrl = reader.GetString(0);
                                apiToken = reader.GetString(1);
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
            }

            return (endpointUrl,apiToken);
        }
        #endregion
    }
}
