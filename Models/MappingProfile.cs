using AutoMapper;

namespace PromptEngineering.Models
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Create a map between Product and ProductDTO
            CreateMap<ChatInput, Chats>();

        }
    }
}
