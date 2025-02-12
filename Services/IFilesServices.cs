using Microsoft.AspNetCore.Http;
using PromptEngineering.Models;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace PromptEngineering.Services
{
    public interface IFilesServices
    {
        Task<string> UploadFile(IFormFile file);
        Task<string> ReadFileContent(string fileName);
        Task<List<FileDetails>> ExtractFiles(Stream fileStream, string zipFileName);
    }
}
