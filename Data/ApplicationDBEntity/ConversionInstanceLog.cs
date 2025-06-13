using System;
using System.ComponentModel.DataAnnotations;
using PromptEngineering.Models.CodeConversion;

namespace PromptEngineering.Data.ApplicationDBEntity
{
    public class ConversionInstanceLog
    {
        [Key]
        public int ConversionInstanceLogId { get; set; }
        public string TitleText { get; set; }
        public string DetailedText { get; set; }
        public ConversionInstanceLogType LogType { get; set; }
        public DateTime Time { get; set; }

        public int ConversionInstanceId {get;set;}
        public ConversionInstance ConversionInstance { get; set; }
    }
}