using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PromptEngineering.Models.CodeConversion
{
    public class ZipUploadResponseModel
    {
        public bool Success {get;set;}
        public int? ConversionInstanceId {get;set;}
        public string? ErrorMessage {get;set;}
    }
}