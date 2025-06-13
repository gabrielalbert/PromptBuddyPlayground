using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PromptEngineering.Utils.SummmaryGenerators.NETSummaryGenerator
{
    public class NETSummaryModel : ISummaryModel
    {
        public string className { get; set; }
        public string methodName { get; set; }
        public string summary { get; set; }

        public bool Equals(ISummaryModel? other)
        {
            var current = (NETSummaryModel)other;
            return className == current.className && methodName == current.methodName;
        }
    }
}