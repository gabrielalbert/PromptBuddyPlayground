using Microsoft.AspNetCore.Http;
using PromptEngineering.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PromptEngineering.Services
{
    public interface IRepoServices
    {
        Task AddRepoFiles(IFormFile[] files, string repoName);
        Task<List<string>> GetAllRepos();
        Task<IEnumerable<RepoSummary>> GetRepoSummary();
        Task<IEnumerable<RepoModel>> GetRepoFiles(string repoName);
    }
}
