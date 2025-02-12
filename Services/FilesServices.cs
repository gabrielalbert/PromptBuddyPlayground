using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Linq;
using System;
using System.Threading.Tasks;
using PromptEngineering.Models;
using Npgsql;
using System.Text;
using System.Collections.Generic;
using System.IO.Compression;

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

        public async Task<string> ReadFileContent(string fileName)
        {
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UploadedFiles", fileName);
            if (!System.IO.File.Exists(filePath))
            {
                throw new FileNotFoundException(" Payload File not Found");
            }

            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                {
                    return await reader.ReadToEndAsync();
                }
            }
        }

        public async Task<List<FileDetails>> ExtractFiles(Stream fileStream, string zipFileName)
        {
            
            string folderPath = $@"{_fileStoragePath}\{zipFileName}";
            _logger.LogInformation($"folderPath {folderPath}");
            
            using var archieve = new ZipArchive(fileStream);
            archieve.ExtractToDirectory(_fileStoragePath, true);

            var files = Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories);
            var fileDetails = new List<FileDetails>();

            foreach (var file in files)
            {
                fileDetails.Add(new FileDetails
                {
                    FileName = Path.GetFileName(file),
                    FileFullPath = Path.GetFullPath(file),
                    FileExtention=Path.GetExtension(file)
                });
            }
            return fileDetails;
        }

    }
}
