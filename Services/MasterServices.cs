using PromptEngineering.Models;
using PromptEngineering.Repository;
using System.Collections.Generic;

namespace PromptEngineering.Services
{
    public class MasterServices:IMasterServices
    {
        private readonly IMasterRepository _masterRepository;        

        public MasterServices( IMasterRepository masterRepository)
        {            
            _masterRepository = masterRepository;
        }

        public List<ProgLangModel> GetProgLangs()
        {
            return _masterRepository.GetProgLangs();
        }
        public List<OfferingModel> GetOfferings()
        {
            return _masterRepository.GetOfferings();
        }
        public List<PhaseModel> GetPhases()
        {
            return _masterRepository.GetPhases();
        }
        public List<RoleModel> GetRoles()
        {
            return _masterRepository.GetRoles();
        }
        public List<UserModel> GetUsers()
        {
            return _masterRepository.GetUsers();
        }
        public List<AiConfigModel> GetAiModels()
        {
            return _masterRepository.GetAiModels();
        }
    }
}
