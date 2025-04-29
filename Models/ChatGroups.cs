using System;

namespace PromptEngineering.Models
{
    public class ChatGroups
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public string GroupDescription { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime LastUpdatedTime { get; set; }
        public int TotalConversations { get; set; }
        
    }
    
}