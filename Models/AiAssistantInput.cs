namespace PromptEngineering.Models
{
    public class AiAssistantInput
    {
        public string LLM { get; set; }
        public string Model { get; set; }
        public string Prompt { get; set; }
        public int GroupID { get; set; }
        public int ConversationId { get; set; }
        public string SelectedRole { get; set; }
        public int UserId { get; set; }
        public string Reference { get; set; }
        public bool FileReference { get; set; } = false;
        public string RepoName { get; set; } = "Public";
    }
}