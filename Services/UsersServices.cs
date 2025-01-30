using AutoMapper;
using PromptEngineering.Models;
using PromptEngineering.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PromptEngineering.Services
{
    public class UsersServices : IUsersServices
    {
        private readonly IUsersRepository _usersRepository;
        private readonly IMapper _mapper;

        public UsersServices(IMapper mapper,IUsersRepository usersRepository)
        {
            _mapper = mapper;
            _usersRepository = usersRepository;
        }

        public async Task<IEnumerable<UserViewModel>> GetUsers()
        {
            return await _usersRepository.GetUsers();
        }

        public async Task<int> CreateUser(UserViewModel user)
        {
            UserModel userM = _mapper.Map<UserModel>(user);
            userM.ManagerId = user.ReportingId;
            int userId= await _usersRepository.CreateUser(userM);
            await _usersRepository.UpdateUserRole(userId, user.RoleId);
            return userId;
        }
        public async Task<int> UpdateUser(UserViewModel user)
        {
            UserModel userM = _mapper.Map<UserModel>(user);
            userM.ManagerId = user.ReportingId;
            await _usersRepository.UpdateUser(userM);
            await _usersRepository.UpdateUserRole(user.UserId, user.RoleId);
            return user.UserId;
        }

        public async Task<int> DeleteUser(int userId)
        {
            return await _usersRepository.DeleteUser(userId);
        }

        public async Task<int> ChangePassword(int userId, string password)
        {
            return await _usersRepository.ChangePassword(userId, password);
        }
        public async Task<bool> ValidateUser(string userName)
        {
            return await _usersRepository.ValidateUser(userName);
        }
        public async Task<IEnumerable<UserModel>> GetReportingToUsers(int userId)
        {
            return await _usersRepository.GetReportingToUsers(userId);
        }

        public async Task<int> UpdateManagers(int userId, int managerId)
        {
            return await _usersRepository.UpdateManagers(userId, managerId);
        }
        public async Task<UserViewModel> GetUser(int userId)
        {
            return await _usersRepository.GetUser(userId);
        }

        public async Task<UserViewModel> LoginUser(string userName, string password)
        {
            return await _usersRepository.LoginUser(userName, password);
        }

    }
}
