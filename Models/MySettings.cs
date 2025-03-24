namespace PromptEngineering.Models
{
    public class MySettings
    {
        public string MaxAttempts { get; set; }
        public string DelayBetweenAttemptInSeconds { get; set; }
        public string RepoUrl { get; set; }
        public string ImageToCodeUrl { get; set; }
        public string LocalModel4UiLayout { get; set; }
        public string ImageModel { get; set; }
        public string ImageApiKey { get; set; }
        public string MaxTimeoutMins { get; set; }
    }
}
