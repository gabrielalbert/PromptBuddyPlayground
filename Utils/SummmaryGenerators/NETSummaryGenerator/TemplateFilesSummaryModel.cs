using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PromptEngineering.Utils.CodeSplitter;

namespace PromptEngineering.Utils.SummmaryGenerators.NETSummaryGenerator
{
    public class TemplateFilesSplitModel : ISplittedCodeModel
    {
        public string fileName { get; set; }
        public string fileContent { get; set; }
    }

    public class TemplateFilesSummaryModel : ISummaryModel
    {
        public string fileName { get; set; }
        public string summary { get; set; }

        public bool Equals(ISummaryModel? other)
        {
            var current = (TemplateFilesSummaryModel)other;
            return fileName == current.fileName;
        }
    }
}