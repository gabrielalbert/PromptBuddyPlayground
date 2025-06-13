using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PromptEngineering.Utils.CodeSplitter
{
    public abstract class CodeSplitterBase<TSplittedCodeModel> where TSplittedCodeModel : ISplittedCodeModel
    {
        public abstract List<TSplittedCodeModel> SplitFile(string fileContent, string fileName);
    }
}