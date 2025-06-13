using System.ComponentModel.DataAnnotations;
using Hangfire.PostgreSql.Properties;

namespace PromptEngineering.Data.ApplicationDBEntity
{
    public class FileEntry
    {
        [Key]
        public int FileEntryId { get; set;}
        [NotNull]
        public string FilePath {get;set;}
        [NotNull]
        public string FileContent {get;set;}
        [NotNull]
        public int FileCollectionId { get; set; }

        // Relationships
        public FileCollection FileCollection {get ; set; }
        public FileEntryMetadata? FileEntryMetadata { get; set; }
    }
}