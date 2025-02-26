using AutoMapper;
using Microsoft.AspNetCore.Http;
using PromptEngineering.Repository;
using System.IO;
using System.Threading.Tasks;
using System;
using PromptEngineering.Models;
using System.Collections.Generic;

namespace PromptEngineering.Services
{
    public class DocsServices: IDocsServices
    {
        private readonly IDocsRepository _docsRepository;        

        public DocsServices(IDocsRepository docsRepository)
        {            
            _docsRepository = docsRepository;
        }

        public async Task<IEnumerable<KTDocsModel>> GetKTDocsSummary()
        {
            return await _docsRepository.GetKTDocsSummary();
        }

    }
}
