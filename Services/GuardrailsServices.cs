using AutoMapper;
using PromptEngineering.Models;
using PromptEngineering.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PromptEngineering.Services
{
    public class GuardrailsServices: IGuardrailsServices
    {
        private readonly IGuardrailsRepository _guardrailsRepository;
        private readonly IMapper _mapper;

        public GuardrailsServices(IMapper mapper, IGuardrailsRepository guardrailsRepository)
        {
            _mapper = mapper;
            _guardrailsRepository = guardrailsRepository;
        }

        public async Task<IEnumerable<GuardrailModel>> GetAll()
        {
            return await _guardrailsRepository.GetAllAsync();
        }

        public async Task<int> AddGuardrail(GuardrailModel guardrail)
        {
            return await _guardrailsRepository.AddAsync(guardrail);
        }

        public async Task<int> UpdateGuardrail(GuardrailModel guardrail)
        {
            return await _guardrailsRepository.UpdateAsync(guardrail);
        }

        public async Task<int> DeleteGuardrail(int id)
        {
            return await _guardrailsRepository.DeleteByIdAsync(id);
        }
    }
}
