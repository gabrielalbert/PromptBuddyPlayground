namespace PromptEngineering.Models
{    
    public class ChatInput
    {
        public string LLM { get; set; }
        public string Model { get; set; }
        public string Language { get; set; }
        public string Phase { get; set; }
        public string PhaseOptional { get; set; }
        public string Prompt { get; set; }
        public string Reference { get; set; }
        public string CurrentUser { get; set; }
        public string SelectedUser { get; set; }
        public string SelectedRole { get; set; }
        public string ConversationId { get; set; }
    }
}
