using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PromptEngineering.Models.DynamicPrompt
{
    public class LLMPrompt
    {
        public string ModelName { get; set; }
        public string SystemPrompt { get; set; }
        public string UserPrompt { get; set; }
        public double? Temperature { get; set; }
        public int? Seed { get; set; }
    }
}