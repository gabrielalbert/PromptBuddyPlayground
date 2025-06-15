using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Features;
using Newtonsoft.Json;

namespace PromptEngineering.Utils.CodeSplitter.CSharpCodeSplitter
{
    public class CSharpCodeSplitterProcessor : CodeSplitterProcessorBase<CSharpSplittedCodeModel>
    {
        public string CodeFileContent { get; init; }
        public CSharpCodeSplitterProcessor(string codeFileContent) {
            CodeFileContent = codeFileContent;
        }

        protected override List<CSharpSplittedCodeModel> GetSplits(List<CodeSplitterData> codeSplitterData)
        {
            return GetMethods(codeSplitterData);
        }
        
        private List<CSharpSplittedCodeModel> GetMethods(List<CodeSplitterData> codeSplitterData, CodeSplitterData classItem = null, CodeSplitterData usingsItem = null, CodeSplitterData namespaceItem = null) 
        {
            var valueDict = GetChunkTypeSplitterData(codeSplitterData);

            var usingsItems = valueDict[CSChunkType.USINGS_DIRECTIVE];
            var nameSpaceNames = valueDict[CSChunkType.NAMESPACE_DECLARATION];
            var constructors = valueDict[CSChunkType.CONSTRUCTOR_DECLARATION];
            var classes = valueDict[CSChunkType.CLASS_DEFINITION];
            var typedMethods = valueDict[CSChunkType.TYPED_MEMBER_DECLARATION];
            var methods =  valueDict[CSChunkType.METHOD_DECLARATION];

            var usingEntryToUse = usingsItem == null ? usingsItems.FirstOrDefault() : usingsItem;
            var namespaceEntryToUse = namespaceItem == null ? nameSpaceNames.FirstOrDefault() : namespaceItem;
            var classEntryToUse = classItem == null ? classes.FirstOrDefault() : classItem;

            var entries = new List<CSharpSplittedCodeModel>();

            foreach(var namespaceEntry in nameSpaceNames) {
                entries.AddRange(GetMethods(namespaceEntry.Children, classEntryToUse, usingEntryToUse, namespaceEntry));
            }

            foreach(var classEntry in classes) {
                entries.AddRange(GetMethods(classEntry.Children, classEntry, usingEntryToUse, namespaceEntryToUse));
            }

            if (classEntryToUse == null || usingEntryToUse == null || namespaceEntryToUse == null) {
                // to prevent interfaces
                return entries;
            }


            foreach(var typedMethod in typedMethods) {
                var insideUntypedMethod = typedMethod.Children.FirstOrDefault(i => i.Type == (int)CSChunkType.METHOD_DECLARATION);
                if (insideUntypedMethod == null) {
                    continue;
                }
                entries.Add(new CSharpSplittedCodeModel {
                    Usings = GetContent(usingEntryToUse),
                    Namespace = GetNamespaceName(namespaceEntryToUse),
                    ClassName = GetClassName(classEntryToUse),
                    MethodContent = "public " + GetContent(typedMethod)
                });
            }

            foreach(var untypedMethod in methods) {
                entries.Add(new CSharpSplittedCodeModel {
                    Usings = GetContent(usingEntryToUse),
                    Namespace = GetNamespaceName(namespaceEntryToUse),
                    ClassName = GetClassName(classEntryToUse),
                    MethodContent = "public void " + GetContent(untypedMethod)
                });
            }

            return entries;
        }

        private string GetContent(CodeSplitterData codeSplitterData) {
            return CodeFileContent.Substring(codeSplitterData.StartIndex, codeSplitterData.EndIndex - codeSplitterData.StartIndex + 1);
        }

        private string GetNamespaceName(CodeSplitterData codeSplitterData) {
            var splitContent = GetContent(codeSplitterData);
            var firstIndex = splitContent.IndexOf(";");
            var secondIndex = splitContent.IndexOf("{");
            var index = Math.Min(firstIndex, secondIndex);
            return splitContent.Substring(0, index).Replace("namespace", "\n").Replace("\n", "").Trim();
        }

        private string GetClassName(CodeSplitterData codeSplitterData) {
            var splitContent = GetContent(codeSplitterData);
            var index = splitContent.IndexOf("{");
            return splitContent.Substring(0, index).Replace("class", "\n").Replace("\n", "").Trim();
        }

        private Dictionary<CSChunkType, List<CodeSplitterData>> GetChunkTypeSplitterData(List<CodeSplitterData> codeSplitterData) {
            var valueDict = new Dictionary<CSChunkType, List<CodeSplitterData>>();
            var arrray = (CSChunkType[])Enum.GetValues(typeof(CSChunkType));
            //var arrray = (CSChunkType[])Enum.GetValuesAsUnderlyingType(typeof(CSChunkType));
            foreach(var val in arrray) {
                valueDict[val] = new List<CodeSplitterData>();
            }
            foreach(var item in codeSplitterData) {
                var chunkType = (CSChunkType)item.Type;
                valueDict[chunkType].Add(item);
            }
            return valueDict;
        }
    }
}