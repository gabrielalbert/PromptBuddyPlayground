using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PromptEngineering.Models.CodeConversion
{
    public class ConversionLogDetailedTextModel
    {
        public string DetailedText { get; set; }
        public ConversionInstanceLogType LogType { get; set; }
    }
}