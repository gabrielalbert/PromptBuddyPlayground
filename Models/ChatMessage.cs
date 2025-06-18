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
        public string RawMessageText { get; set; }
        public int GroupId { get; set; }
        public ReplyMessage Reply { get; set; }
    }

    public class ReplyMessage
    {
        public int ReplyId { get; set; }
        public string ReplyText { get; set; }
        public string ReplyDate { get; set; }

    }
}
