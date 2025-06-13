using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PromptEngineering.Models.CodeConversion
{
    public class ConversionLogModel
    {
        public int ConversionInstanceLogId { get; set; }
        public string TitleText { get; set; }
        public int LogType { get; set; }
        public DateTime Time { get; set; }
        public int ConversionInstanceId {get;set;}
    }
}