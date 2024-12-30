using System;

namespace PromptEngineering.Models
{
    public class ChatMessage
    {
        public int ChatId { get; set; }
        public int MessageId { get; set; }
        public string MessageText { get; set; }        
        public string MessageSender { get; set; }
        public DateTime MessageDate { get; set; }        
        public string PromptEnggType { get; set; }
        public string Feedback {get;set;}
    }
}
