using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;
using PromptEngineering.Models;
using PromptEngineering.Utils;
using System;
using System.Collections.Generic;

namespace PromptEngineering.Repository
{
    public class MasterRepository:IMasterRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<MasterRepository> _logger;

        public MasterRepository(ILogger<MasterRepository> logger, IConfiguration configuration)
        {
            _logger = logger;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public List<ProgLangModel> GetProgLangs()
        {   
            var progLangs = new List<ProgLangModel>();
            
            try
            {
                var sql = @"select lang_id,lang_name,lang_type from languages";                
                sql += " order by lang_id asc;";
                
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new NpgsqlCommand(sql, connection))
                    {                        
                        using (var reader = command.ExecuteReader())
                        {                            
                            while (reader.Read())
                            {                                
                                var pLang = new ProgLangModel();
                                pLang.LangId = reader.GetInt32(0);
                                pLang.LangName = (reader.IsDBNull(1) ? string.Empty : reader.GetString(1));
                                pLang.LangType = reader.GetString(2);
                                progLangs.Add(pLang);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
            }
            return progLangs;
        }
        public List<OfferingModel> GetOfferings()
        {
            var offerings = new List<OfferingModel>();

            try
            {
                var sql = @"select offering_id,offering_name from offerings";
                sql += " order by offering_id asc;";

                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var offeringM = new OfferingModel();
                                offeringM.OfferingId = reader.GetInt32(0);
                                offeringM.OfferingName = (reader.IsDBNull(1) ? string.Empty : reader.GetString(1));                                
                                offerings.Add(offeringM);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
            }
            return offerings;
        }
        public List<PhaseModel> GetPhases()
        {
            var phases = new List<PhaseModel>();

            try
            {
                var sql = @"select phase_id,phase_name from phases";
                sql += " order by phase_id asc;";

                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var phaseM = new PhaseModel();
                                phaseM.PhaseId = reader.GetInt32(0);
                                phaseM.PhaseName = (reader.IsDBNull(1) ? string.Empty : reader.GetString(1));
                                phases.Add(phaseM);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
            }
            return phases;
        }

        public List<RoleModel> GetRoles()
        {
            var roles = new List<RoleModel>();

            try
            {
                var sql = @"select role_id,role_name,is_manager,is_admin from roles";
                sql += " order by role_id asc;";

                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var roleM = new RoleModel();
                                roleM.RoleId = reader.GetInt32(0);
                                roleM.RoleName = (reader.IsDBNull(1) ? string.Empty : reader.GetString(1));
                                roleM.IsManager = reader.IsDBNull(2)?false:reader.GetBoolean(2);
                                roleM.IsAdmin = reader.IsDBNull(3) ? false : reader.GetBoolean(3); 
                                roles.Add(roleM);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
            }
            return roles;
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
                                userM.Email = (reader.IsDBNull(5)? string.Empty:reader.GetString(5));
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
