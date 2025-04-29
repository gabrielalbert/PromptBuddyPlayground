using PromptEngineering.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PromptEngineering.Services
{
    public interface IGuardrailsServices
    {
        Task<IEnumerable<GuardrailModel>> GetAll();
        Task<int> AddGuardrail(GuardrailModel guardrail);
        Task<int> UpdateGuardrail(GuardrailModel guardrail);
        Task<int> DeleteGuardrail(int id);
    }
}
