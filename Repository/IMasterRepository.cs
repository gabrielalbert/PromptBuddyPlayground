using PromptEngineering.Models;
using System.Collections.Generic;

namespace PromptEngineering.Repository
{
    public interface IMasterRepository
    {
        List<ProgLangModel> GetProgLangs();
        List<OfferingModel> GetOfferings();
        List<PhaseModel> GetPhases();
        List<RoleModel> GetRoles();
        List<UserModel> GetUsers();
        List<AiConfigModel> GetAiModels();
    }
}
