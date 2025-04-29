using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;
using PromptEngineering.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace PromptEngineering.Repository
{
    public class GuardrailsRepository:IGuardrailsRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<GuardrailsRepository> _logger;
        public GuardrailsRepository(ILogger<GuardrailsRepository> logger, IConfiguration configuration)
        {
            _logger = logger;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<GuardrailModel>> GetAllAsync()
        {
            var guards = new List<GuardrailModel>();

            try
            {
                var sql = @"SELECT * FROM Guardrails";

                using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();
                using var command = new NpgsqlCommand(sql, connection);
                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    GuardrailModel guard = GetGuardrailModel(reader);
                    guards.Add(guard);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
            }
            return guards;
        }


        public async Task<IEnumerable<GuardrailModel>> GetByIdsAsync(string guardRailIds)
        {
            var guards = new List<GuardrailModel>();

            try
            {
                var sql = $"SELECT * FROM Guardrails where id in ({guardRailIds})";

                using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();
                using var command = new NpgsqlCommand(sql, connection);
                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    GuardrailModel guard = GetGuardrailModel(reader);
                    guards.Add(guard);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
            }
            return guards;
        }

        public async Task<int> DeleteByIdAsync(int id)
        {
            var result = 0;
            try
            {
                var sql = @"DELETE FROM Guardrails WHERE id=@id;";
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        result = (int)await command.ExecuteScalarAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
            }
            return result;
        }

        public async Task<int> AddAsync(GuardrailModel guardrail)
        {
            var result = 0;
            try
            {
                var sql = @"insert into Guardrails(name,modelname,Description) values(@name,@modelname,@description) RETURNING id;";
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@name", guardrail.Name);
                        command.Parameters.AddWithValue("@modelname", guardrail.ModelName);
                        command.Parameters.AddWithValue("@description", guardrail.Description);
                        result = (int)await command.ExecuteScalarAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
            }
            return result;
        }

        public async Task<int> UpdateAsync(GuardrailModel guardrail)
        {
            var result = 0;
            try
            {
                var sql = @"update Guardrails set name=@name,modelname=@modelname,Description=@description,
                createdat=current_timestamp where id=@user_id;";
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@name", guardrail.Name);
                        command.Parameters.AddWithValue("@modelname", guardrail.ModelName);
                        command.Parameters.AddWithValue("@description", guardrail.Description);
                        result = await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
            }
            return result;
        }

        private GuardrailModel GetGuardrailModel(NpgsqlDataReader reader)
        {
            var guard = new GuardrailModel();
            guard.Id = reader.GetInt32(0);
            guard.Name = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);
            guard.ModelName = reader.IsDBNull(2) ? string.Empty : reader.GetString(2);
            guard.Description = reader.IsDBNull(3) ? string.Empty : reader.GetString(3);
            guard.CreatedAt = reader.IsDBNull(4) ? DateTime.Now : reader.GetDateTime(4);
            return guard;
        }
    }
}
