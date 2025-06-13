using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PromptEngineering.Data.ApplicationDBEntity;
using PromptEngineering.Models.CodeConversion;
using PromptEngineering.Models.DynamicPrompt;
using PromptEngineering.Services.CodeConversion;
using PromptEngineering.Utils.CodeSplitter;

namespace PromptEngineering.Utils.SummmaryGenerators
{
    public abstract class SummaryGeneratorBase<TSplittedCodeModel, TSummaryJsonModel> where TSummaryJsonModel : ISummaryModel where TSplittedCodeModel : ISplittedCodeModel
    {
        private ILLMService LLMService;
        private int conversionInstanceId;
        private IConversionLogService ConversionLogService;

        public SummaryGeneratorBase(ILLMService llMService, IConversionLogService conversionLogService, int conversionInstanceId)
        {
            LLMService = llMService;
            this.conversionInstanceId = conversionInstanceId;
            this.ConversionLogService = conversionLogService;
        }

        protected abstract Task<LLMPrompt> GetSummaryGenerationPrompt(TSplittedCodeModel splittedCodeModel, List<TSummaryJsonModel> summaryJsonModel);
        protected abstract List<TSummaryJsonModel> GenerateSummaryModel(string promptResponse, TSplittedCodeModel splittedCodeModel);

        public async Task<List<TSummaryJsonModel>> GetSummaryJsonModel(List<FileEntry> fileEntries, CodeSplitterBase<TSplittedCodeModel> codeSplitter)
        {
            var splittedCodeModels = new List<TSplittedCodeModel>();
            foreach (var fileEntry in fileEntries)
            {
                var splittedCodeFileModels = codeSplitter.SplitFile(fileEntry.FileContent, fileEntry.FilePath);
                splittedCodeModels.AddRange(splittedCodeFileModels);
            }
            await ConversionLogService.Log($"'{splittedCodeModels.Count}' Code splits generated...", "", ConversionInstanceLogType.INFO, conversionInstanceId);
            return await GetSummaryJsonModel(splittedCodeModels);
        }

        private async Task<List<TSummaryJsonModel>> GetSummaryJsonModel(List<TSplittedCodeModel> splittedCodeModels)
        {
            var allSummaryModels = new List<TSummaryJsonModel>();
            var summaryModels = new List<TSummaryJsonModel>();

            int summaryLoopIndex = 1;
            foreach (var splittedCode in splittedCodeModels)
            {
                var prompt = await GetSummaryGenerationPrompt(splittedCode, summaryModels);
                await ConversionLogService.Log($"Prompting code block {summaryLoopIndex}/{splittedCodeModels.Count}", prompt.UserPrompt, ConversionInstanceLogType.PROMPT_SENDING, conversionInstanceId);
                var response = await LLMService.GetResponseDirect(prompt);
                await ConversionLogService.Log($"Prompted code block {summaryLoopIndex}/{splittedCodeModels.Count}", response.Response, ConversionInstanceLogType.PROMPT_SUCCESS, conversionInstanceId);
                summaryModels = GenerateSummaryModel(response.Response, splittedCode);
                foreach (var model in summaryModels)
                {
                    if (allSummaryModels.FirstOrDefault(r => r.Equals(model)) == null)
                    {
                        allSummaryModels.Add(model);
                    }
                }
                summaryLoopIndex++;
            }
            await ConversionLogService.Log($"Summary json generated...", JsonConvert.SerializeObject(allSummaryModels), ConversionInstanceLogType.SUCCESS, conversionInstanceId);
            return allSummaryModels;
        }
    }
}