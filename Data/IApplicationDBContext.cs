using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PromptEngineering.Data.ApplicationDBEntity;
using PromptEngineering.Models.CodeConversion;

namespace PromptEngineering.Data
{
    public interface IApplicationDBContext
    {
        Task<bool> AddConversionInstance(ConversionInstance conversionInstance);
        Task<List<ConversionInstance>> GetConversionInstances();
        Task<ConversionInstance> GetConversionInstance(int conversionInstanceId);
        Task<List<FileEntry>> GetFileEntries(int fileCollectionId);
        Task<FileEntry> GetFileEntry(int fileEntryId);
        Task<string> GetFileEntryContent(int fileEntryId);

        Task<bool> UpdateConversionInstanceStatus(int conversionInstanceId, ConversionStatusEnum conversionStatusEnum);
        Task<bool> UpdateConversionInstanceCodeSplitterJSON(int conversionInstanceId, string splittedFileJSON);

        Task<IQueryable<ConversionInstanceLog>> GetConversionInstanceLogs(int conversionInstanceId);
        Task<IQueryable<ConversionInstanceLog>> GetConversionInstanceLogsAfter(int conversionInstanceId, int conversionInstanceLogId);
        Task<ConversionInstanceLog> GetConversionInstanceDetailedLog(int conversionInstanceLogId);

        Task<bool> AddConversionInstanceLog(ConversionInstanceLog conversionInstanceLog);
        Task<bool> AddConversionInstanceLog(int conversionInstanceId, string titleText, string detailedText, ConversionInstanceLogType conversionInstanceLogType);
        Task<bool> UpdateConversionesultFileCollection(int conversionInstanceId, FileCollection fileCollection);

        Task<IQueryable<DynamicPrompt>> GetAllDynamicPrompts();
        Task<DynamicPrompt> GetDynamicPrompt(int dynamicPromptId);
        Task<DynamicPrompt> GetDynamicPrompt(string uniqueName);
        Task<bool> AddUpdateDynamicPrompt(DynamicPrompt dynamicPrompt);

    }
}