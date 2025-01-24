using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;
using NpgsqlTypes;
using PromptEngineering.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Threading.Tasks;

namespace PromptEngineering.Repository
{
    public class UsersRepository : IUsersRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<UsersRepository> _logger;
        public UsersRepository(ILogger<UsersRepository> logger, IConfiguration configuration)
        {
            _logger = logger;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public async Task<IEnumerable<UserViewModel>> GetUsers()
        {
            var users = new List<UserViewModel>();

            try
            {
                var sql = @"select u.user_id,u.user_name,u.display_name,COALESCE(u.manager_id,0) as reporting_id,u.password,u.email
                            , COALESCE(m.display_name,'') as reporting_to, ur.role_id, r.role_name from users u
                               LEFT JOIN users m ON u.manager_id = m.user_id
                               LEFT JOIN user_role_mapping ur ON u.user_id = ur.user_id
                               LEFT JOIN roles r ON ur.role_id = r.role_id
                             order by u.user_id asc;";

                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var userM = new UserViewModel();
                                userM.UserId = reader.GetInt32(0);
                                userM.UserName = (reader.IsDBNull(1) ? string.Empty : reader.GetString(1));
                                userM.DisplayName = (reader.IsDBNull(2) ? string.Empty : reader.GetString(2));
                                userM.ReportingId = reader.IsDBNull(3) ? 0 : reader.GetInt32(3);
                                userM.Password = (reader.IsDBNull(4) ? string.Empty : reader.GetString(4));
                                userM.Email = (reader.IsDBNull(5) ? string.Empty : reader.GetString(5));
                                userM.ReportingTo = (reader.IsDBNull(6) ? string.Empty : reader.GetString(6));
                                userM.RoleId = reader.IsDBNull(7) ? 0 : reader.GetInt32(7);
                                userM.RoleName = (reader.IsDBNull(8) ? string.Empty : reader.GetString(8));
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

        public async Task<int> CreateUser(UserModel user)
        {
            var result = 0;
            try
            {
                var sql = @"insert into users(user_name,display_name,manager_id,password,email) values(@user_name,@display_name,@manager_id,@password,@email)  RETURNING user_id;";
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@user_name", user.UserName);
                        command.Parameters.AddWithValue("@display_name", string.IsNullOrEmpty(user.DisplayName) ? DBNull.Value : user.DisplayName);
                        command.Parameters.AddWithValue("@manager_id", (user.ManagerId == 0 ? DBNull.Value : user.ManagerId));
                        command.Parameters.AddWithValue("@password", user.Password);
                        command.Parameters.AddWithValue("@email", user.Email);
                        result = (int) await command.ExecuteScalarAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
            }
            return result;
        }

        public async Task<int> UpdateUser(UserModel user)
        {
            var result = 0;
            try
            {
                var sql = @"update users set user_name=@user_name,display_name=@display_name,manager_id=@manager_id,password=@password,email=@email where user_id=@user_id;";
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@user_id", user.UserId);
                        command.Parameters.AddWithValue("@user_name", user.UserName);
                        command.Parameters.AddWithValue("@display_name", string.IsNullOrEmpty(user.DisplayName) ? DBNull.Value : user.DisplayName);
                        command.Parameters.AddWithValue("@manager_id", (user.ManagerId == 0 ? DBNull.Value : user.ManagerId));
                        command.Parameters.AddWithValue("@password", user.Password);
                        command.Parameters.AddWithValue("@email", user.Email);
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

        public async Task<int> DeleteUser(int userId)
        {
            var result = 0;
            try
            {
                var sql = @"deleteuser";
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("p_userid", userId);
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

        public async Task<int> ChangePassword(int userId, string password)
        {
            var result = 0;
            try
            {
                var sql = @"update users set password=@password where user_id=@user_id;";
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@user_id", userId);
                        command.Parameters.AddWithValue("@password", password);
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

        public async Task<bool> ValidateUser(string userName)
        {
            var result = false;
            try
            {
                var sql = @"select count(*) from users where user_name=@user_name;";
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@user_name", userName);
                        var count = Convert.ToInt32(await command.ExecuteScalarAsync());
                        result = count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
            }
            return result;
        }

        public async Task<IEnumerable<UserModel>> GetReportingToUsers(int userId)
        {
            var users = new List<UserModel>();

            try
            {
                var sql = @"select user_id,user_name,display_name,manager_id,password,email from users";
                sql += (userId > 0 ? " where user_id!=@user_id" : string.Empty) + " order by user_id asc;";

                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        if (userId > 0)
                        {
                            command.Parameters.AddWithValue("@user_id", userId);
                        }
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
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

        public async Task<int> UpdateManagers(int userId, int managerId)
        {
            var result = 0;
            try
            {
                var sql = @"update users set manager_id=@manager_id where user_id=@user_id;";
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@user_id", userId);
                        command.Parameters.AddWithValue("@manager_id", managerId);
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

        public async Task<UserViewModel> GetUser(int userId)
        {
            var userM = new UserViewModel();

            try
            {
                var sql = @"select u.user_id,u.user_name,u.display_name,COALESCE(u.manager_id,0) as reporting_id,u.password,u.email
                            , COALESCE(m.display_name,'') as reporting_to, ur.role_id, r.role_name from users u
                               LEFT JOIN users m ON u.manager_id = m.user_id
                               LEFT JOIN user_role_mapping ur ON u.user_id = ur.user_id
                               LEFT JOIN roles r ON ur.role_id = r.role_id";
                sql += " where u.user_id=@user_id;";

                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@user_id", userId);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                userM.UserId = reader.GetInt32(0);
                                userM.UserName = (reader.IsDBNull(1) ? string.Empty : reader.GetString(1));
                                userM.DisplayName = (reader.IsDBNull(2) ? string.Empty : reader.GetString(2));
                                userM.ReportingId = reader.IsDBNull(3) ? 0 : reader.GetInt32(3);
                                userM.Password = (reader.IsDBNull(4) ? string.Empty : reader.GetString(4));
                                userM.Email = (reader.IsDBNull(5) ? string.Empty : reader.GetString(5));
                                userM.ReportingTo = (reader.IsDBNull(6) ? string.Empty : reader.GetString(6));
                                userM.RoleId = reader.IsDBNull(7) ? 0 : reader.GetInt32(7);
                                userM.RoleName = (reader.IsDBNull(8) ? string.Empty : reader.GetString(8));

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
            return userM;

        }

        public async Task<int> UpdateUserRole(int userId, int roleId)
        {
            var result = 0;
            try
            {
                var sql = @"updateuserandrole";
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("p_user_id", NpgsqlDbType.Integer, userId);
                        command.Parameters.AddWithValue("p_role_id", NpgsqlDbType.Integer, roleId);                        
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
    }
}
