namespace PromptEngineering.Models
{
    public class RepoSummary
    {
        public string RepoName { get; set; }
        public long FileCount { get; set; }
        public bool Indexed { get; set; }
    }
}
