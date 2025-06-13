using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PromptEngineering.Utils.SummmaryGenerators.NETSummaryGenerator
{
    public class CombinedSummaryModel
    {
        public List<NETSummaryModel> methodSummaries { get; set; } 
        public List<TemplateFilesSummaryModel> templateSummaries { get; set; }
    }
}