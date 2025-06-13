using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PromptEngineering.Models.CodeConversion
{
    public class FileCollectionModel
    {
        public int FileCollectionId {get;set;}
        public List<FileEntryModel> Files { get; set; }
    }
}