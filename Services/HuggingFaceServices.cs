using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PromptEngineering.Models;
using PromptEngineering.Repository;
using System;

namespace PromptEngineering.Services
{
    public class HuggingFaceServices:IHuggingFaceServices
    {
        private readonly IChatRepository _chatRepository;
        private readonly ILogger<HuggingFaceServices> _logger;
        private readonly IMapper _mapper;        
        private string outputFolder = AppDomain.CurrentDomain.BaseDirectory + Configurations.OUTPUT_FOLDER;
        
        public HuggingFaceServices(ILogger<HuggingFaceServices> logger, IMapper mapper, IChatRepository chatRepository)
        {
            _logger = logger;
            _mapper = mapper;
            _chatRepository = chatRepository;           
        }


    }
}
