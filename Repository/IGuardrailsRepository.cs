using System.Collections.Generic;
using System.Threading.Tasks;
using PromptEngineering.Models;

namespace PromptEngineering.Repository
{
    public interface IGuardrailsRepository
    {
        Task<IEnumerable<GuardrailModel>> GetAllAsync();
        Task<IEnumerable<GuardrailModel>> GetByIdsAsync(string guardrailIds);
        Task<int> AddAsync(GuardrailModel guardrail);
        Task<int> UpdateAsync(GuardrailModel guardrail);
        Task<int> DeleteByIdAsync(int id);
    }
}
