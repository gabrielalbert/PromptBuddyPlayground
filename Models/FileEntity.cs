using System;
using Microsoft.VisualBasic;

namespace PromptEngineering.Models
{
    public class FileEntity
    {
        public string FileName { get; set; }
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    }
}
