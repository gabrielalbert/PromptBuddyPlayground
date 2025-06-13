using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PromptEngineering.Utils.CodeSplitter.CSharpCodeSplitter
{
    public class CSharpSplittedCodeModel : ISplittedCodeModel
    {
        public string Usings { get; set; }
        public string Namespace { get; set; }
        public string ClassName { get; set; }
        public string MethodContent { get; set; }
    } 
}