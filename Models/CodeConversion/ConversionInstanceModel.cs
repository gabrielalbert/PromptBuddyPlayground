using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PromptEngineering.Models.CodeConversion
{
    public class ConversionInstanceModel
    {
        public int ConversionInstanceId { get; set; }
        public string Guid { get; set; }
        public string FriendlyName {get;set;}
        public int FileCollectionId { get; set; }
        public int? ResultFileCollectionId { get; set; }
        public int ConversionStatus { get; set; }
    }
}