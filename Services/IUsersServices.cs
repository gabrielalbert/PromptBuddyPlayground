using PromptEngineering.Models;
using System.Collections.Generic;

namespace PromptEngineering.Services
{
    public interface IUsersServices
    {
        List<UserModel> GetUsers();
    }
}
