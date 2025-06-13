using PromptEngineering.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PromptEngineering.Repository
{
    public interface ILayoutRepository
    {
        Task<int> AddUiLayoutEntry(LayoutModel input);
        Task<List<LayoutModel>> GetAllUiLayoutDatas();
        Task<LayoutModel> GetUiLayoutDatas(int id);

        Task UpdateUiLayoutEntryStatus(int id,string status, string userName);
        Task UpdateResult(int id, string result, string userName);
    }
}
