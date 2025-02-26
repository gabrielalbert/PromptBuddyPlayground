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
    public class DocsRepository : IDocsRepository
    {

        private readonly string _connectionString;
        private readonly ILogger<DocsRepository> _logger;
        public DocsRepository(ILogger<DocsRepository> logger, IConfiguration configuration)
        {
            _logger = logger;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<KTDocsModel>> GetKTDocsSummary()
        {
            var docsSummaries = new List<KTDocsModel>();
            try
            {
                var sql = @"select FileName,FullFileName,UploadedOn,CompletedOn,Status from get_docs_summary()";

                _logger.LogInformation($"GetKTDocsSummary using Function");

                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var docsSummary = new KTDocsModel
                                {
                                    FileName = (reader.IsDBNull(0) ? string.Empty : reader.GetString(0)),
                                    FullFileName = (reader.IsDBNull(1) ? string.Empty : reader.GetString(1)),
                                    UploadedOn = (reader.IsDBNull(2) ? new DateTime() : reader.GetDateTime(2)),
                                    CompletedOn = (reader.IsDBNull(3) ? new DateTime() : reader.GetDateTime(3)),
                                    Status = (reader.IsDBNull(4) ? string.Empty : reader.GetString(4)),
                                };

                                docsSummaries.Add(docsSummary);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
            }
            return docsSummaries;
        }
    }
}
