using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualBasic;

namespace PromptEngineering.Utils.CodeSplitter
{
    public abstract class CodeSplitterProcessorBase<T>
    {
        private Dictionary<SplitterKey, SplitterValue> SplitterStoreDict = new Dictionary<SplitterKey, SplitterValue>();

        public void InjestStart(int type, int start) {
            SplitterStoreDict[new SplitterKey(type, start)] = new SplitterValue(start, null);
        }

        public void InjestEnd(int type, int start, int end) {
            SplitterStoreDict[new SplitterKey(type, start)] = new SplitterValue(start, end);
        }

        private List<CodeSplitterData> Process() {
            var codeSplits = new List<CodeSplitterData>();
            foreach(var pair in SplitterStoreDict) {
                codeSplits.Add(new CodeSplitterData {
                    Type = pair.Key.type,
                    StartIndex = (int)pair.Value.startIndex.Value,
                    EndIndex = (int)pair.Value.stopIndex.Value,
                    Children = new List<CodeSplitterData>()
                });
            }
            return RecursiveChildrenGenerator(codeSplits);
        }

        private List<CodeSplitterData> RecursiveChildrenGenerator(List<CodeSplitterData> inputDataList) {
            if (inputDataList == null || inputDataList.Count == 0) {
                return inputDataList;
            }
            var returnList = new List<CodeSplitterData>();
            var remainingItems = inputDataList;
            bool running = true;
            do {
                var item = remainingItems.First();
                var insideItemsList = inputDataList.Where(i => i != item).Where(i => i.StartIndex >= item.StartIndex && i.EndIndex <= item.EndIndex).ToList();              
                var outsideItemsList = inputDataList.Where(i => i != item).Except(insideItemsList).ToList();
                item.Children = RecursiveChildrenGenerator(insideItemsList);
                remainingItems = RecursiveChildrenGenerator(remainingItems.Except(insideItemsList).Where(i => i != item).ToList());
                returnList.Add(item);
            } while(remainingItems.Count > 0);
            return returnList;
        }

        public List<T> GetSplittedCode() {
            return GetSplits(Process());
        }

        protected abstract List<T> GetSplits(List<CodeSplitterData> codeSplitterData);

        public class CodeSplitterData {
            public int Type { get; set; }
            public int StartIndex { get; set; }
            public int EndIndex { get; set; }
            public List<CodeSplitterData> Children { get; set; } = new List<CodeSplitterData>();
        }

        public record class SplitterKey(int type, int startIndex);
        public record class SplitterValue(int? startIndex, int? stopIndex);
    }
}