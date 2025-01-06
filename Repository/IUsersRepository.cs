using PromptEngineering.Models;
using System.Collections.Generic;

namespace PromptEngineering.Repository
{
    public interface IUsersRepository
    {
        List<UserModel> GetUsers();
    }
}
