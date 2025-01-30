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
    public class RepoServices: IRepoServices
    {
        private readonly IRepoRepository _repoRepository;
        private readonly IMapper _mapper;        
        private readonly string _filesStoragePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UploadedRepoFiles");

        public RepoServices(IMapper mapper, IRepoRepository repoRepository)
        {
            _mapper = mapper;
            _repoRepository = repoRepository;
        }

        public async Task AddRepoFiles(IFormFile[] files, string repoName)
        {
            var repoPath = Path.Combine(_filesStoragePath, repoName);
            if (!Directory.Exists(repoPath))
            {
                Directory.CreateDirectory(repoPath);
            }
            foreach (var file in files)
            {                
                await SaveRepoFileToLocal(repoName,repoPath,file);
            }
        }

        private async Task SaveRepoFileToLocal(string repoName,string filePath, IFormFile file)
        {
            var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var addRepoFilePath = Path.Combine(filePath, uniqueFileName);
            using (var stream = new FileStream(addRepoFilePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            var fileData = new RepoModel
            {
                RepoName = repoName,
                OriginalFileName = file.FileName,
                UniqueFileName = uniqueFileName,
                Indexed = false,
                Timestamp = DateTime.UtcNow
            };
            await _repoRepository.AddRepoFiles(fileData);
        }

        public async Task<List<string>> GetAllRepos()
        {
            return await _repoRepository.GetAllRepos();
        }

        public async Task<IEnumerable<RepoSummary>> GetRepoSummary()
        {
            return await _repoRepository.GetRepoSummary();
        }

        public async Task<IEnumerable<RepoModel>> GetRepoFiles(string repoName)
        {
            return await _repoRepository.GetRepoFiles(repoName);
        }
    }
}
