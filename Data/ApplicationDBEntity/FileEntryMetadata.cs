using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Hangfire.PostgreSql.Properties;

namespace PromptEngineering.Data.ApplicationDBEntity
{
    public class FileEntryMetadata
    {
        public int FileEntryMetadataId { get; set; }
        
        [NotNull]
        public int FileEntryId { get; set;}

        [NotNull]
        public string MetadataJSON { get; set; }
    }
}