using System;

namespace PromptEngineering.Models
{
    public class Chats
    {
        public string MessageId { get; set; }
        public string ConversationId { get; set; }
        public string LLM { get; set; }
        public string Model { get; set; }
        public string Language { get; set; }
        public string Phase { get; set; }
        public string PhaseOptional { get; set; }
        public string Prompt { get; set; }
        public string Reference { get; set; }
        public string Result { get; set; }
        public string Status { get; set; }
        public int Attempt { get; set; }
        public bool Success { get; set; }
        public bool AI { get; set; }
        public DateTime RequestedTime { get; set; }
        public DateTime RespondedTime { get; set; }
        public string PromptEnggType { get; set; }
        public int InputTokens { get; set; }
        public int OutputTokens { get; set; }
        public int TotalTokens { get; set; }
        public string SelectedUser { get; set; }
        public string SelectedRole { get; set; }

    }
}
