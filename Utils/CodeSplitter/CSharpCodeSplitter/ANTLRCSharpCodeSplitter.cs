using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Newtonsoft.Json;

namespace PromptEngineering.Utils.CodeSplitter.CSharpCodeSplitter
{
    public class ANTLRCSharpCodeSplitter : CodeSplitterBase<CSharpSplittedCodeModel>
    {
        public override List<CSharpSplittedCodeModel> SplitFile(string fileContent, string fileName)
        {
            var inputStream = new AntlrInputStream(fileContent);
            var lexer = new CSharpLexer(inputStream);
            var tokens = new CommonTokenStream(lexer);
            var parser = new CSharpParser(tokens);

            var splitter = new CSharpCodeSplitterProcessor(fileContent);
            var listener = new CodeSplitterCSharpListener(splitter);
            var tree = parser.compilation_unit();
            ParseTreeWalker.Default.Walk(listener, tree);

            var list = splitter.GetSplittedCode();
            return list;
        }
    }
}