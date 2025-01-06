using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;
using PromptEngineering.Models;
using System;
using System.Collections.Generic;

namespace PromptEngineering.Repository
{
    public class UsersRepository:IUsersRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<UsersRepository> _logger;
        public UsersRepository(ILogger<UsersRepository> logger, IConfiguration configuration)
        {
            _logger = logger;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public List<UserModel> GetUsers()
        {
            var users = new List<UserModel>();

            try
            {
                var sql = @"select user_id,user_name,display_name,manager_id,password,email from users";
                sql += " order by user_id asc;";

                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var userM = new UserModel();
                                userM.UserId = reader.GetInt32(0);
                                userM.UserName = (reader.IsDBNull(1) ? string.Empty : reader.GetString(1));
                                userM.DisplayName = (reader.IsDBNull(2) ? string.Empty : reader.GetString(2));
                                userM.ManagerId = reader.IsDBNull(3) ? 0 : reader.GetInt32(3);
                                userM.Password = (reader.IsDBNull(4) ? string.Empty : reader.GetString(4));
                                userM.Email = (reader.IsDBNull(5) ? string.Empty : reader.GetString(5));
                                users.Add(userM);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
            }
            return users;
        }
    }
}
