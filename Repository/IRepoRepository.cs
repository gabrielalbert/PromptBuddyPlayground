using PromptEngineering.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PromptEngineering.Repository
{
    public interface IRepoRepository
    {
        Task AddRepoFiles(RepoModel fileData);
        Task<List<string>> GetAllRepos();
        Task<IEnumerable<RepoSummary>> GetRepoSummary();
        Task<IEnumerable<RepoModel>> GetRepoFiles(string repoName);
        

    }
}
