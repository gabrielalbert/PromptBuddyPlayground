using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using PromptEngineering.Utils.SummmaryGenerators.NETSummaryGenerator;

namespace PromptEngineering.Utils.CodeSplitter.CSharpCodeSplitter
{
    public class TemplateCSharpCodeSplitter: CodeSplitterBase<TemplateFilesSplitModel>
    {
        public override List<TemplateFilesSplitModel> SplitFile(string fileContent, string fileName)
        {
            var fileInfo = new FileInfo(fileName);
            return new List<TemplateFilesSplitModel>
            {
                new TemplateFilesSplitModel {
                    fileName = fileInfo.Name,
                    fileContent = fileContent
                }
            };
        }
    }
}