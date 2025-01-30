using PromptEngineering.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PromptEngineering.Repository
{
    public interface IUsersRepository
    {
        Task<IEnumerable<UserViewModel>> GetUsers();
        Task<int> CreateUser(UserModel user);
        Task<int> UpdateUser(UserModel user);
        Task<int> DeleteUser(int userId);
        Task<int> ChangePassword(int userId, string password);
        Task<bool> ValidateUser(string userName);
        Task<IEnumerable<UserModel>> GetReportingToUsers(int userId);
        Task<int> UpdateManagers(int userId, int managerId);
        Task<UserViewModel> GetUser(int userId);
        Task<int> UpdateUserRole(int userId, int roleId);
        Task<UserViewModel> LoginUser(string userName, string password);

    }
}
