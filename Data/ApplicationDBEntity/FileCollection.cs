using System.Collections.Generic;

namespace PromptEngineering.Data.ApplicationDBEntity
{
    public class FileCollection
    {
        public int FileCollectionId {get;set;}
        // Relationships
        public List<FileEntry> FileEntries {get;set;}
    }
}