using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Hangfire.PostgreSql.Properties;
using PromptEngineering.Models.CodeConversion;

namespace PromptEngineering.Data.ApplicationDBEntity
{
    public class ConversionInstance
    {
        [Key]
        public int ConversionInstanceId { get; set; }
        [NotNull]
        public Guid Guid { get; set; }
        [NotNull]
        public string FriendlyName {get;set;}
        [NotNull]
        public int FileCollectionId { get; set; }
        public string? CodeSplitterJSON { get; set; }
        public int? ResultFileCollectionId { get; set; }
        [NotNull]
        public ConversionStatusEnum ConversionStatus { get; set; }
        public FileCollection FileCollection { get; set; }
        public List<ConversionInstanceLog> ConversionInstanceLogs {get;set;}
    }
}