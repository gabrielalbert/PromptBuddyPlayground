using Microsoft.AspNetCore.Http;
using PromptEngineering.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PromptEngineering.Services
{
    public interface IDocsServices
    {
        Task<IEnumerable<KTDocsModel>> GetKTDocsSummary();
    }
}
