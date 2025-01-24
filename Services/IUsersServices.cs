using PromptEngineering.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PromptEngineering.Services
{
    public interface IUsersServices
    {
        Task<IEnumerable<UserViewModel>> GetUsers();
        Task<int> CreateUser(UserViewModel user);
        Task<int> UpdateUser(UserViewModel user);
        Task<int> DeleteUser(int userId);
        Task<int> ChangePassword(int userId, string password);
        Task<bool> ValidateUser(string userName);
        Task<IEnumerable<UserModel>> GetReportingToUsers(int userId);
        Task<int> UpdateManagers(int userId, int managerId);
        Task<UserViewModel> GetUser(int userId);
    }
}
