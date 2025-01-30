using System;

namespace PromptEngineering.Models
{
    public class RepoModel
    {
        public string RepoName { get; set; }
        public string OriginalFileName { get; set; }
        public string UniqueFileName { get; set; }
        public bool Indexed { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
