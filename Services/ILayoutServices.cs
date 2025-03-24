using PromptEngineering.Models;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace PromptEngineering.Services
{
    public interface ILayoutServices
    {
        
        Task<string> AddUiLayoutEntry(LayoutModel input);
        Task<List<LayoutModel>> GetAllUiLayoutDatas();
        Task<string> StartProcess(int layoutId, string userName);
        Task<(string,string)> DownloadFile(int layoutId);
        Task<string> GetImageContent(int layoutId);
    }
}
