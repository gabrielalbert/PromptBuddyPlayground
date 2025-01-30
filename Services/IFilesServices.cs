using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace PromptEngineering.Services
{
    public interface IFilesServices
    {
        Task<string> UploadFile(IFormFile file);
        Task<string> ReadFileContent(string fileName);
    }
}
