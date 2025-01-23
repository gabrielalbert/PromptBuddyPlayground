using PromptEngineering.Models;
using System.Threading.Tasks;

namespace PromptEngineering.Services
{
    public interface ICopilotCLIServices
    {
        Task<Chats> ExecuteCopilotCommand(Chats chats, bool proxyEnabled = true);
    }
}
