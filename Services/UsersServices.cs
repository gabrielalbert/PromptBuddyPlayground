using PromptEngineering.Models;
using PromptEngineering.Repository;
using System.Collections.Generic;

namespace PromptEngineering.Services
{
    public class UsersServices:IUsersServices
    {
        private readonly IUsersRepository _usersRepository;

        public UsersServices(IUsersRepository usersRepository)
        {
            _usersRepository = usersRepository;
        }

        public List<UserModel> GetUsers()
        {
            return _usersRepository.GetUsers();
        }
    }
}
