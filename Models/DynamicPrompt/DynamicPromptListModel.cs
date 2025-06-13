using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PromptEngineering.Models.DynamicPrompt
{
    public class DynamicPromptListModel
    {
        public int DynamicPromptId { get; set; }
        public string UniqueName { get; set; }
        public string Description{ get; set; }
    }
}