using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PromptEngineering.Models.DynamicPrompt
{
    public class DynamicPromptModel
    {
        public int DynamicPromptId { get; set; }
        public string UniqueName { get; set; }
        public string Description { get; set; }
        public string SystemPromptTemplate { get; set; }
        public List<string> SystemPromptKeys { get; set; }
        public string UserPromptTemplate { get; set; }
        public List<string> UserPromptKeys { get; set; }
        public string ModelName { get; set; }
        public int? Temperature { get; set; }
        public int? Seed { get; set; }
    }
}