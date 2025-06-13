using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Hangfire;
using Microsoft.AspNetCore.SignalR;
using System.IO.Compression;
using Newtonsoft.Json;
using System.Text;
using PromptEngineering.Hubs;
using PromptEngineering.Services.CodeConversion;
using PromptEngineering.Data;
using PromptEngineering.Models.CodeConversion;
using PromptEngineering.Data.ApplicationDBEntity;
using System.IO;
using PromptEngineering.Background;

namespace ConversionServer.Controllers
{
    [ApiController]
    [Route("api/conversion")]
    public class CodeConversionController : ControllerBase
    {
        private ITaskUpdateService TaskUpdateService { get; set; }
        private ILLMService LLMService { get; set; }
        private IApplicationDBContext ApplicationDBContext { get; set; }

        public CodeConversionController(
            ITaskUpdateService taskUpdateService,
            ILLMService llmService,
            IApplicationDBContext applicationDBContext
        )
        {
            TaskUpdateService = taskUpdateService;
            LLMService = llmService;
            ApplicationDBContext = applicationDBContext;
        }

        [HttpPost]
        [Route("zipUploadStart")]
        public async Task<ZipUploadResponseModel> InitiateConversion(ZipUploadModel uploadModel)
        {
            using var uploadZipFileStream = uploadModel.ZipFile.OpenReadStream();
            using var archive = new ZipArchive(uploadZipFileStream);
            var zipFileEntries = archive.Entries;
            if (zipFileEntries.Count == 0)
            {
                return new ZipUploadResponseModel
                {
                    Success = false,
                    ErrorMessage = "No files in zip file"
                };
            }
            var conversionInstance = new ConversionInstance();
            conversionInstance.Guid = Guid.NewGuid();
            conversionInstance.FriendlyName = uploadModel.FriendlyName;
            var fileCollection = new FileCollection();
            fileCollection.FileEntries = new List<FileEntry>();
            foreach (var zipFileEntry in zipFileEntries)
            {
                var fileInfo = new System.IO.FileInfo(zipFileEntry.FullName);
                // for csharp
                if (fileInfo.Directory.FullName.Contains("obj") || fileInfo.Directory.FullName.Contains("bin"))
                {
                    continue;
                }
                //
                if (fileInfo.Extension == "")
                {
                    continue;
                }
                using var zipFileEntryStream = zipFileEntry.Open();
                using var streamReader = new StreamReader(zipFileEntryStream);
                var fileContent = await streamReader.ReadToEndAsync();
                var fileEntry = new FileEntry
                {
                    FilePath = zipFileEntry.FullName,
                    FileContent = fileContent
                };
                fileCollection.FileEntries.Add(fileEntry);
            }
            conversionInstance.FileCollection = fileCollection;
            conversionInstance.ConversionStatus = ConversionStatusEnum.FILES_ADD_COMPLETE;
            var success = await ApplicationDBContext.AddConversionInstance(conversionInstance);
            await ApplicationDBContext.AddConversionInstanceLog(
                conversionInstance.ConversionInstanceId,
                "Files added to store",
                "", ConversionInstanceLogType.INFO
            );
            var generateProjectSummaryId = BackgroundJob.Enqueue<ProjectConversionNewBackground>((background) =>
                background.GenerateProjectSummaryJson(conversionInstance.ConversionInstanceId)
            );
            BackgroundJob.ContinueJobWith<ProjectConversionNewBackground>(generateProjectSummaryId, (background) =>
                background.GenerateFinalProjectFiles(conversionInstance.ConversionInstanceId)
            );
            return new ZipUploadResponseModel
            {
                Success = success,
                ErrorMessage = null,
                ConversionInstanceId = conversionInstance.ConversionInstanceId
            };
        }

        [HttpGet]
        [Route("getConversionInstances")]
        public async Task<List<ConversionInstanceModel>> GetConversionInstances()
        {
            return (await ApplicationDBContext.GetConversionInstances()).Select(e => new ConversionInstanceModel
            {
                ConversionInstanceId = e.ConversionInstanceId,
                Guid = e.Guid.ToString(),
                FriendlyName = e.FriendlyName,
                ConversionStatus = (int)e.ConversionStatus,
                FileCollectionId = e.FileCollectionId,
                ResultFileCollectionId = e.ResultFileCollectionId
            }).ToList();
        }

        [HttpGet]
        [Route("getInstanceFileCollection")]
        public async Task<FileCollectionModel> GetFileCollectionEntries(int fileCollectionId)
        {
            var fileEntries = await ApplicationDBContext.GetFileEntries(fileCollectionId);
            return new FileCollectionModel
            {
                FileCollectionId = fileCollectionId,
                Files = fileEntries.Select(e => new FileEntryModel
                {
                    FileEntryId = e.FileEntryId,
                    FilePath = e.FilePath
                }).ToList()
            };
        }

        [HttpGet]
        [Route("getInstanceFileEntryContent")]
        public async Task<FileContentModel> GetFileContent(int fileEntryId)
        {
            var fileEntry = await ApplicationDBContext.GetFileEntry(fileEntryId);
            return new FileContentModel
            {
                FileEntryId = fileEntryId,
                FileContent = fileEntry.FileContent
            };
        }

        [HttpGet]
        [Route("getInstanceLogs")]
        public async Task<List<ConversionLogModel>> GetConversionInstanceLogs(int conversionInstanceId)
        {
            return (await ApplicationDBContext.GetConversionInstanceLogs(conversionInstanceId)).Select(e => new ConversionLogModel
            {
                ConversionInstanceId = e.ConversionInstanceId,
                ConversionInstanceLogId = e.ConversionInstanceLogId,
                Time = e.Time,
                TitleText = e.TitleText,
                LogType = (int)e.LogType
            }).ToList();
        }

        [HttpGet]
        [Route("getInstanceLogsAfter")]
        public async Task<List<ConversionLogModel>> GetConversionInstanceLogsAfter(int conversionInstanceId, int conversionInstanceLogId)
        {
            return (await ApplicationDBContext.GetConversionInstanceLogsAfter(conversionInstanceId, conversionInstanceLogId)).Select(e => new ConversionLogModel
            {
                ConversionInstanceId = e.ConversionInstanceId,
                ConversionInstanceLogId = e.ConversionInstanceLogId,
                Time = e.Time,
                TitleText = e.TitleText,
                LogType = (int)e.LogType
            }).ToList();
        }

        [HttpGet]
        [Route("getInstanceLogDetailedText")]
        public async Task<ConversionLogDetailedTextModel> GetConversionInstancLogsDetailedText(int conversionInstanceLogId)
        {
            var conversionInstanceLog = await ApplicationDBContext.GetConversionInstanceDetailedLog(conversionInstanceLogId);
            return new ConversionLogDetailedTextModel
            {
                DetailedText = conversionInstanceLog.DetailedText,
                LogType = conversionInstanceLog.LogType
            };
        }

        [HttpGet]
        [Route("downloadZipFile")]
        public async Task<IActionResult> DownloadZipFile(int fileCollectionId)
        {
            var fileEntryCollection = await ApplicationDBContext.GetFileEntries(fileCollectionId);
            byte[] fileBytes = new byte[0];
            using (var memorystream = new MemoryStream())
            {
                using (var archive = new ZipArchive(memorystream, ZipArchiveMode.Create))
                {
                    foreach (var fileEntry in fileEntryCollection)
                    {
                        var fileContent = await ApplicationDBContext.GetFileEntryContent(fileEntry.FileEntryId);
                        ZipArchiveEntry readmeEntry = archive.CreateEntry(fileEntry.FilePath);
                        using (StreamWriter writer = new StreamWriter(readmeEntry.Open()))
                        {
                            writer.Write(fileContent);
                        }
                    }
                }
                fileBytes = memorystream.ToArray();
            }
            return File(fileBytes, "applization/zip", $"convertedFiles{fileCollectionId}.zip");
        }
    }
}