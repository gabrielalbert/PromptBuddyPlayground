using Npgsql;
using PromptEngineering.Models;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PromptEngineering.Repository
{
    public class LayoutRepository:ILayoutRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<LayoutRepository> _logger;
        
        public LayoutRepository(ILogger<LayoutRepository> logger, IConfiguration configuration)
        {
            _logger = logger;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public async Task<int> AddUiLayoutEntry(LayoutModel input)
        {
            int uilayoutId = 0;
            try
            {
                var sql = @"select insert_uilayoutdata(@p_image_file,@p_filename,@p_frontend_language,@p_css_framework,@p_layout_name,@p_custom_prompt,@p_output_filename,@p_status,@p_uploaded_by,@p_uploaded_at)";

                _logger.LogInformation($"SaveUiLayoutEntryAsync Query: {sql} ");

                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@p_image_file", input.ImageFileName);
                        command.Parameters.AddWithValue("@p_filename", input.FileName);
                        command.Parameters.AddWithValue("@p_frontend_language", input.Frontend);
                        command.Parameters.AddWithValue("@p_css_framework", input.CssFramework);
                        command.Parameters.AddWithValue("@p_layout_name", (string.IsNullOrEmpty(input.LayoutName)?string.Empty:input.LayoutName));                       
                        command.Parameters.AddWithValue("@p_custom_prompt", (string.IsNullOrEmpty(input.CustomizePrompt) ? string.Empty : input.CustomizePrompt));
                        command.Parameters.AddWithValue("@p_output_filename", (string.IsNullOrEmpty(input.LayoutName) ? string.Empty : Regex.Replace(input.LayoutName, @"[^a-zA-Z0-9]", "")));                        
                        command.Parameters.AddWithValue("@p_status", input.Status);
                        command.Parameters.AddWithValue("@p_uploaded_by", input.UploadedBy);
                        command.Parameters.AddWithValue("@p_uploaded_at", input.UploadedOn);

                        // Execute the command and get the returned value
                        uilayoutId = (int)await command.ExecuteScalarAsync();
                        _logger.LogInformation("Files successfully inserted.");
                    }
                }                
                
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
            }
            return uilayoutId;
        }

        public async Task<List<LayoutModel>> GetAllUiLayoutDatas()
        {
            var layoutList = new List<LayoutModel>();
            try
            {
                var sql = @$"select id,image_file,frontend_language,css_framework,layout_name,status,uploaded_at,processed_at,output_filename from uilayouts;";

                _logger.LogInformation($"GetAllUiLayoutDatas query{sql}");

                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                layoutList.Add(new LayoutModel
                                {
                                    Id = reader.GetInt32(0),
                                    ImageFileName = reader.IsDBNull(1) ? "" : reader.GetString(1),
                                    Frontend = reader.IsDBNull(2) ? "" : reader.GetString(2),
                                    CssFramework = reader.IsDBNull(3) ? "" : reader.GetString(3),
                                    LayoutName = reader.IsDBNull(4) ? "" : reader.GetString(4),
                                    Status =reader.IsDBNull(5) ? "" : reader.GetString(5),
                                    UploadedOn = reader.GetDateTime(6),
                                    ProcessedOn = reader.GetDateTime(7),
                                    OutputFileName = reader.IsDBNull(8)?"": reader.GetString(8)
                                });                                
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
            }
            return layoutList;
        }

        public async Task<LayoutModel> GetUiLayoutDatas(int id)
        {
            var layout = new LayoutModel();
            try
            {
                var sql = @$"select id,image_file,frontend_language,css_framework,layout_name,status,uploaded_at,processed_at,custom_prompt,response,output_filename,file_name from uilayouts where id=@id;";

                _logger.LogInformation($"GetUiLayoutDatas query{sql}");

                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {

                                layout.Id = reader.GetInt32(0);
                                layout.ImageFileName = (reader.IsDBNull(1) ? "" : reader.GetString(1));
                                layout.Frontend = (reader.IsDBNull(2) ? "" : reader.GetString(2));
                                layout.CssFramework = (reader.IsDBNull(3) ? "" : reader.GetString(3));
                                layout.LayoutName = (reader.IsDBNull(4) ? "" : reader.GetString(4));
                                layout.Status = (reader.IsDBNull(5) ? "" : reader.GetString(5));
                                layout.UploadedOn = reader.IsDBNull(6) ? default(DateTime) : reader.GetDateTime(6);
                                layout.ProcessedOn = reader.IsDBNull(7)? default(DateTime):reader.GetDateTime(7);
                                layout.CustomizePrompt = (reader.IsDBNull(8) ? "" : reader.GetString(8)); 
                                layout.ResponseMessage = (reader.IsDBNull(9) ?"": reader.GetString(9));
                                layout.OutputFileName = (reader.IsDBNull(10) ? "" : reader.GetString(10));
                                layout.FileName = (reader.IsDBNull(11) ? "" : reader.GetString(11));
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
            return layout;
        }


        public async Task UpdateUiLayoutEntryStatus(int id, string status, string userName)        {
            
            try
            {
                var sql = @$"update uilayouts set status=@p_status,processed_by=@p_processed_by, processed_at=@p_processed_at where id=(@p_id);";
               
                _logger.LogInformation($"UpdateUiLayoutEntryStatus Query: {sql} ");

                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@p_id", id);
                        command.Parameters.AddWithValue("@p_status", status);
                        command.Parameters.AddWithValue("@p_processed_by", userName);
                        command.Parameters.AddWithValue("@p_processed_at", DateTime.Now);

                        // Execute the command and get the returned value
                        await command.ExecuteNonQueryAsync();
                        _logger.LogInformation("Status updated successfully.");
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
            }           
        }


        public async Task UpdateResult(int id, string result, string userName)
        {

            try
            {
                var sql = @$"update uilayouts set response=@p_response, status='completed',processed_by=@p_processed_by, processed_at=@p_processed_at where id=(@p_id);";

                _logger.LogInformation($"UpdateUiLayoutEntryStatus Query: {sql} ");

                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@p_id", id);
                        command.Parameters.AddWithValue("@p_response", result);
                        command.Parameters.AddWithValue("@p_processed_by", userName);
                        command.Parameters.AddWithValue("@p_processed_at", DateTime.Now);

                        // Execute the command and get the returned value
                        await command.ExecuteNonQueryAsync();
                        _logger.LogInformation("Status updated successfully.");
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
            }
        }


        public (string endpointUrl, string apiToken) GetAiEndpointForImageToLayoutUrl()
        {

            var endpointUrl = string.Empty;
            var apiToken = string.Empty;

            string llm_key = "copilot|gpt-4o";
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

            return (endpointUrl, apiToken);
        }
    }
}
