using System;

namespace PromptEngineering.Models
{
    public class KTDocsModel
    {
        public string FileName { get; set; }
        public string FullFileName { get; set; }
        public DateTime UploadedOn { get; set; }
        public DateTime CompletedOn { get; set; }

        public string Status { get; set; }
    }
}
