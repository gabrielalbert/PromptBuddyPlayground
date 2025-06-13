using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PromptEngineering.Models.DynamicPrompt
{
    public class PromptTestResponse
    {
        public string Response { get; set; }
        public List<int> Context { get; set; }
        public int Duration { get; set; }
        public bool Error { get; set; }
    }
}