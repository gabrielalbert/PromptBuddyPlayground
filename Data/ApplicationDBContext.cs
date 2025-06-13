using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PromptEngineering.Data.ApplicationDBEntity;
using PromptEngineering.Models.CodeConversion;

namespace PromptEngineering.Data
{
    public class ApplicationDBContext : DbContext, IApplicationDBContext
    {
        private IConfiguration configuration;
        
        public ApplicationDBContext(IConfiguration configRoot)
        {
            configuration = configRoot;
        }

        public DbSet<FileCollection> FileCollections { get; set; }
        public DbSet<FileEntry> FileEntries { get; set; }
        public DbSet<FileEntryMetadata> FileEntryMetadatas {get;set;}
        
        public DbSet<ConversionInstance> ConversionInstances { get; set; }
        public DbSet<ConversionInstanceLog> ConversionInstanceLogs { get; set; }
        public DbSet<DynamicPrompt> DynamicPrompts { get; set; }

        public async Task<bool> AddConversionInstance(ConversionInstance conversionInstance) {
            await ConversionInstances.AddAsync(conversionInstance);
            await SaveChangesAsync();
            return true;
        }

        public async Task<List<ConversionInstance>> GetConversionInstances() {
            return await ConversionInstances
                .OrderBy(e => e.ConversionInstanceId)
                .ToListAsync();
        }

        public async Task<ConversionInstance> GetConversionInstance(int conversionInstanceId) {
            return await ConversionInstances.SingleAsync(e => e.ConversionInstanceId == conversionInstanceId);
        }

        public async Task<List<FileEntry>> GetFileEntries(int fileCollectionId) {
            return await FileEntries
                .Where(e => e.FileCollectionId == fileCollectionId)
                .OrderBy(e => e.FileEntryId)
                .ToListAsync();
        }

        public async Task<FileEntry> GetFileEntry(int fileEntryId) {
            return await FileEntries.SingleAsync(e => e.FileEntryId == fileEntryId);
        }

        public async Task<string> GetFileEntryContent(int fileEntryId) {
            var fileEntry = await FileEntries.SingleAsync(e => e.FileEntryId == fileEntryId);
            return fileEntry.FileContent;
        }

        public async Task<bool> UpdateConversionInstanceStatus(int conversionInstanceId, ConversionStatusEnum conversionStatus) {
            var conversionInstance = await ConversionInstances.SingleAsync(e => e.ConversionInstanceId == conversionInstanceId);
            conversionInstance.ConversionStatus = conversionStatus;
            await this.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateConversionInstanceCodeSplitterJSON(int conversionInstanceId, string splittedFileJSON) {
            var conversionInstance = await ConversionInstances.SingleAsync(e => e.ConversionInstanceId == conversionInstanceId);
            conversionInstance.CodeSplitterJSON = splittedFileJSON;
            await this.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AddConversionInstanceLog(ConversionInstanceLog conversionInstanceLog) {
            await ConversionInstanceLogs.AddAsync(conversionInstanceLog);
            await this.SaveChangesAsync();
            return true;
        }

        public Task<bool> AddConversionInstanceLog(int conversionInstanceId, string titleText, string detailedText, ConversionInstanceLogType conversionInstanceLogType) {
            var conversionInstanceLog = new ConversionInstanceLog {
                TitleText = titleText,
                DetailedText = detailedText,
                LogType = conversionInstanceLogType,
                Time = DateTime.UtcNow,
                ConversionInstanceId = conversionInstanceId
            };
            return AddConversionInstanceLog(conversionInstanceLog);
        }

        public async Task<IQueryable<ConversionInstanceLog>> GetConversionInstanceLogs(int conversionInstanceId) {
            return ConversionInstanceLogs
                .Where(e => e.ConversionInstanceId == conversionInstanceId)
                .OrderByDescending(e => e.ConversionInstanceLogId);
        }

        public async Task<IQueryable<ConversionInstanceLog>> GetConversionInstanceLogsAfter(int conversionInstanceId, int conversionInstanceLogId) {
            return ConversionInstanceLogs
                .Where(e => e.ConversionInstanceId == conversionInstanceId && e.ConversionInstanceLogId > conversionInstanceLogId)
                .OrderByDescending(e => e.ConversionInstanceLogId);
        }

        public async Task<ConversionInstanceLog> GetConversionInstanceDetailedLog(int conversionInstanceLogId) {
            return await ConversionInstanceLogs.FirstOrDefaultAsync(e => e.ConversionInstanceLogId == conversionInstanceLogId);
        }

        public async Task<bool> UpdateConversionesultFileCollection(int conversionInstanceId, FileCollection fileCollection) {
            await FileCollections.AddAsync(fileCollection);
            await this.SaveChangesAsync();
            var conversionInstance = await ConversionInstances.SingleAsync(e => e.ConversionInstanceId == conversionInstanceId);
            conversionInstance.ResultFileCollectionId = fileCollection.FileCollectionId;
            conversionInstance.ConversionStatus = ConversionStatusEnum.DONE_CREATING_FILES_COMPLETE;
            await this.SaveChangesAsync();
            return true;
        }

        public async Task<IQueryable<DynamicPrompt>> GetAllDynamicPrompts()
        {
            return DynamicPrompts;    
        }

        public async Task<DynamicPrompt> GetDynamicPrompt(int dynamicPromptId)
        {
            return await DynamicPrompts.FirstOrDefaultAsync(e => e.DynamicPromptId == dynamicPromptId);
        }

        public async Task<DynamicPrompt> GetDynamicPrompt(string uniqueName)
        {
            return await DynamicPrompts.FirstOrDefaultAsync(e => e.UniqueName == uniqueName);
        }

        public async Task<bool> AddUpdateDynamicPrompt(DynamicPrompt dynamicPrompt)
        {
            var isPromptExisting = await DynamicPrompts.AnyAsync(e => e.DynamicPromptId == dynamicPrompt.DynamicPromptId);
            if (isPromptExisting)
            {
                DynamicPrompts.Update(dynamicPrompt);
                await this.SaveChangesAsync();
                return true;
            }
            DynamicPrompts.Add(dynamicPrompt);
            await this.SaveChangesAsync();
            return true;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            optionsBuilder.UseNpgsql(connectionString);
        }
    }
}