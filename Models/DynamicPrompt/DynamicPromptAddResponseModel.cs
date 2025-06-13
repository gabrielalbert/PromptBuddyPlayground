using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PromptEngineering.Models.DynamicPrompt
{
    public class DynamicPromptAddResponseModel
    {
        public bool Success { get; set; }
        public List<string> ErrorMessages { get; set; }
    }
}