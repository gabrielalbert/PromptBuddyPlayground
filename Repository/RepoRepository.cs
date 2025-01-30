using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;
using System;
using Npgsql;
using PromptEngineering.Models;
using System.Collections.Generic;

namespace PromptEngineering.Repository
{
    public class RepoRepository:IRepoRepository    {
        
        private readonly string _connectionString;
        private readonly ILogger<RepoRepository> _logger;
        public RepoRepository(ILogger<RepoRepository> logger, IConfiguration configuration)
        {
            _logger = logger;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        #region AddRepoFiles

        public async Task AddRepoFiles(RepoModel fileData)
        {
            try
            {
                var sql = @"call InsertAddRepoFilesData(@p_reponame,@p_original_filename,@p_unique_filename,@p_indexed)";

                _logger.LogInformation($"SaveFilesDataToDBAsync Query {sql} ");

                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@p_reponame", fileData.RepoName);
                        command.Parameters.AddWithValue("@p_original_filename", fileData.OriginalFileName);
                        command.Parameters.AddWithValue("@p_unique_filename", fileData.UniqueFileName);
                        command.Parameters.AddWithValue("@p_indexed", fileData.Indexed);

                        await command.ExecuteNonQueryAsync();

                        _logger.LogInformation("Files successfully inserted.");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
            }
        }

        #endregion

        public async Task<List<string>> GetAllRepos()
        {
            var reposList = new List<string>();
            try
            {
                var sql = @"select * from GetAllRepos()";

                _logger.LogInformation($"GetUniqueRepoNamesAsync using Function");

                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                reposList.Add(reader.GetString(0));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
            }
            return reposList;
        }

        public async Task<IEnumerable<RepoSummary>> GetRepoSummary()
        {
            var repoSummaries = new List<RepoSummary>();
            try
            {
                var sql = @"select * from get_repository_summary()";

                _logger.LogInformation($"GetRepoSummaryAsync using Function");

                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var repoSummary = new RepoSummary
                                {
                                    RepoName = (reader.IsDBNull(0) ? string.Empty : reader.GetString(0)),
                                    FileCount = (reader.IsDBNull(1) ? 0 : reader.GetInt64(1)),
                                    Indexed = (reader.IsDBNull(2) ? false : reader.GetBoolean(2))
                                };

                                repoSummaries.Add(repoSummary);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
            }
            return repoSummaries;
        }

        public async Task<IEnumerable<RepoModel>> GetRepoFiles(string repoName)
        {
            var files = new List<RepoModel>();
            try
            {
                var sql = @"select * from get_files_repository(@RepoName)";

                _logger.LogInformation($"GetFilesByRepo using Function");

                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@RepoName", repoName);
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                files.Add(new RepoModel
                                {
                                    RepoName = (reader.IsDBNull(0) ? string.Empty : reader.GetString(0)),
                                    OriginalFileName = (reader.IsDBNull(1) ? string.Empty : reader.GetString(1)),
                                    UniqueFileName = (reader.IsDBNull(2) ? string.Empty : reader.GetString(2)),
                                    Indexed = (reader.IsDBNull(3) ? false : reader.GetBoolean(3)),
                                    Timestamp = (reader.IsDBNull(4) ? new DateTime() : reader.GetDateTime(4))
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
            return files;
        }

    }
}
