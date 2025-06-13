using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PromptEngineering.Data;
using PromptEngineering.Data.ApplicationDBEntity;
using PromptEngineering.Hubs;
using PromptEngineering.Models.CodeConversion;
using PromptEngineering.Services.CodeConversion;
using PromptEngineering.Utils.CodeSplitter.CSharpCodeSplitter;
using PromptEngineering.Utils.SummmaryGenerators.NETSummaryGenerator;

namespace PromptEngineering.Background
{
    public class ProjectConversionNewBackground
    {
        private ITaskUpdateService TaskUpdateService;
        private ILLMService LLMServiceInstance;
        private IApplicationDBContext ApplicationDBContext;
        private IConversionLogService ConversionLogService;
        private IDynamicPromptService DynamicPromptService;


        public ProjectConversionNewBackground(
            ITaskUpdateService taskUpdateService,
            IConversionLogService conversionLogService,
            ILLMService llMService,
            IApplicationDBContext applicationDBContext,
            IDynamicPromptService dynamicPromptService
        )
        {
            TaskUpdateService = taskUpdateService;
            ConversionLogService = conversionLogService;
            LLMServiceInstance = llMService;
            ApplicationDBContext = applicationDBContext;
            DynamicPromptService = dynamicPromptService;
        }

        public async Task GenerateProjectSummaryJson(int conversionInstanceId) {
            var conversionInstance = await ApplicationDBContext.GetConversionInstance(conversionInstanceId);
            var fileEntries = await ApplicationDBContext.GetFileEntries(conversionInstance.FileCollectionId);
            var fileEntriesToConsider = fileEntries.Where(entry => {
                var fileInfo = new FileInfo(entry.FilePath);
                return fileInfo.Extension == ".cs";
            }).ToList();

            var summaryGenerator = new NETSummaryGenerator(LLMServiceInstance, ConversionLogService, conversionInstanceId, DynamicPromptService);
            var allIterativeModels = await summaryGenerator.GetSummaryJsonModel(fileEntriesToConsider, new ANTLRCSharpCodeSplitter());

            fileEntriesToConsider = fileEntries.Where(entry => {
                var fileInfo = new FileInfo(entry.FilePath);
                return fileInfo.Extension == ".cshtml" || fileInfo.Extension == ".html";
            }).ToList();

            var templateSummaryGenerator = new TemplateSummaryGenerator(LLMServiceInstance, ConversionLogService, conversionInstanceId, DynamicPromptService);
            var allTemplateIterativeModels = await templateSummaryGenerator.GetSummaryJsonModel(fileEntriesToConsider, new TemplateCSharpCodeSplitter());

            await ApplicationDBContext.UpdateConversionInstanceCodeSplitterJSON(conversionInstanceId, JsonConvert.SerializeObject(new CombinedSummaryModel { methodSummaries = allIterativeModels, templateSummaries = allTemplateIterativeModels }));
            await ApplicationDBContext.UpdateConversionInstanceStatus(conversionInstanceId, ConversionStatusEnum.ANALYSIS_COMPLETE);
            await TaskUpdateService.SendConversionInstanceStatus(conversionInstanceId, (int)ConversionStatusEnum.ANALYSIS_COMPLETE);
        }

        public async Task GenerateFinalProjectFiles(int conversionInstanceId) {
            var conversionInstance = await ApplicationDBContext.GetConversionInstance(conversionInstanceId);
            var finalAnalysisJSON = conversionInstance.CodeSplitterJSON;
            var iterativeModel = JsonConvert.DeserializeObject<CombinedSummaryModel> (finalAnalysisJSON);
            
            var generateProjectPrompt = await DynamicPromptService.GetDynamicPrompt(
                "csharpProjectFilesFromJSONSummaryPrompt",
                new Dictionary<string, string>
                {
                    { "jsonSummaries", JsonConvert.SerializeObject(iterativeModel, Formatting.Indented) }
                },
                new Dictionary<string, string>()
            );
    
            await AddPromptLog("Prompting final code generation", conversionInstanceId, generateProjectPrompt.UserPrompt);
            var response = await LLMServiceInstance.GetResponseDirect(generateProjectPrompt);
            await AddPromptFinishLog("Prompting final code generation finished", conversionInstanceId, response.Response, ConversionInstanceLogType.PROMPT_FINAL_SUCCESS);
            var finalResponse = response.Response;
            var result = GenerateAllResultFilesFromResponse(finalResponse);

            if (result.Keys.Count == 0) {
                return;
            }

            var fileCollection = new FileCollection { FileEntries = new List<FileEntry>() };
            foreach(var entry in result) {
                var fileEntry = new FileEntry {
                    FilePath = entry.Key,
                    FileContent = entry.Value,
                };
                fileCollection.FileEntries.Add(fileEntry);
            }
            await ApplicationDBContext.UpdateConversionesultFileCollection(conversionInstanceId, fileCollection);
            await TaskUpdateService.SendConversionInstanceDoneStatus(
                conversionInstanceId,
                (int)ConversionStatusEnum.DONE_CREATING_FILES_COMPLETE,
                fileCollection.FileCollectionId
            );
        }
        
        private Dictionary<string, string> GenerateAllResultFilesFromResponse(string response)
        {
            var resultDict = new Dictionary<string, string>();
            var regex = new Regex("##[^`]+```");
            var matches = regex.Matches(response);
            if (matches.Count == 0)
            {
                return resultDict;
            }
            var matchIndexes = matches.Select(m => new { index = m.Index, str = m.ToString() }).ToList();
            foreach (var match in matchIndexes)
            {
                var stringToWorkOn = response.Substring(match.index);
                var fileName = match.str.Replace("##", "").Replace("```", "").Trim();
                stringToWorkOn = stringToWorkOn.Replace(match.str, "");
                int indexOfNext = stringToWorkOn.IndexOf("```");
                
                var currentBlock = stringToWorkOn.Substring(0, indexOfNext);
                currentBlock = currentBlock.Replace("```", "");
                var firstLineEnd = currentBlock.IndexOf("\n");
                currentBlock = currentBlock.Substring(firstLineEnd);

                resultDict.Add(fileName, currentBlock);
            }
            return resultDict;
        }

        private async Task AddPromptLog(string titleText, int conversionInstanceId, string userPromptText) {
            await ApplicationDBContext.AddConversionInstanceLog(
               conversionInstanceId,
                titleText,
                $"{userPromptText}", ConversionInstanceLogType.PROMPT_SENDING
            );
            await TaskUpdateService.SendConversionInstanceLogsAdded(conversionInstanceId);
        }

        private async Task AddPromptFinishLog(string titleText, int conversionInstanceId, string response, ConversionInstanceLogType conversionInstanceLogType) {
            await ApplicationDBContext.AddConversionInstanceLog(
               conversionInstanceId,
                titleText,
                $"{response}", conversionInstanceLogType
            );
            await TaskUpdateService.SendConversionInstanceLogsAdded(conversionInstanceId);
        }
    }
}