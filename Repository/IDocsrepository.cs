using PromptEngineering.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PromptEngineering.Repository
{
    public interface IDocsRepository
    {
        Task<IEnumerable<KTDocsModel>> GetKTDocsSummary();
    }
}
