using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Linq;
using System;
using System.Threading.Tasks;
using PromptEngineering.Models;
using Npgsql;

namespace PromptEngineering.Services
{
    public class FilesServices: IFilesServices
    {
        private readonly ILogger<FilesServices> _logger;
        private readonly string _fileStoragePath;

        public FilesServices(ILogger<FilesServices> logger)
        {
            _fileStoragePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory , "UploadedFiles");
            if (!Directory.Exists(_fileStoragePath))
            {
                Directory.CreateDirectory(_fileStoragePath);
            }
        }
        private readonly string[] _supportedFileTypes = new[]{
            ".txt",".doc",".pdf",".docx",".cs",".json",".xml",".html",".css",".js",".ts"
        };              

        public async Task<string> UploadFile(IFormFile file)
        {
            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            if (!_supportedFileTypes.Contains(fileExtension))
            {
                return "File type not supported";
            }

            var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
            var fileEntity = new FileEntity { FileName = uniqueFileName };

            using (var ms = new MemoryStream())
            {
                await file.CopyToAsync(ms);
                var filePath = Path.Combine(_fileStoragePath, fileEntity.FileName);
                await File.WriteAllBytesAsync(filePath, ms.ToArray());                
            }
            return uniqueFileName;
        }        
    }
}
