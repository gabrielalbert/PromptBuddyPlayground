using PromptEngineering.Models;
using System.Collections.Generic;

namespace PromptEngineering.Services
{
    public interface IMasterServices
    {
        List<ProgLangModel> GetProgLangs();
        List<OfferingModel> GetOfferings();
        List<PhaseModel> GetPhases();
        List<RoleModel> GetRoles();
        List<UserModel> GetUsers();
    }
}
