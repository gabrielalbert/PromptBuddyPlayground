using Microsoft.AspNetCore.Http;
using System;

namespace PromptEngineering.Models
{
    public class LayoutModel
    {
        public int Id { get; set; }
        public string ImageFileName { get; set; }
        public string FileName { get; set; }            
        public string Frontend { get; set; }
        public string CssFramework { get; set; }
        public string LayoutName { get; set; }
        public string CustomizePrompt { get; set; }
        public DateTime UploadedOn { get; set; }
        public string UploadedBy { get; set; }
        public DateTime ProcessedOn { get; set; }
        public string ProcessedBy { get; set; }
        public string Status { get; set; }
        public string ImageFile { get; set; } 
        public string ResponseMessage { get; set; }
        public string OutputFileName { get; set; }

    }
}
