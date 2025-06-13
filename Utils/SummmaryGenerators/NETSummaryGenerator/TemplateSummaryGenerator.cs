using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PromptEngineering.Models.DynamicPrompt;
using PromptEngineering.Services.CodeConversion;

namespace PromptEngineering.Utils.SummmaryGenerators.NETSummaryGenerator
{
    public class TemplateSummaryGenerator : SummaryGeneratorBase<TemplateFilesSplitModel, TemplateFilesSummaryModel>
    {
        private IDynamicPromptService DynamicPromptService { get; set; }

        public TemplateSummaryGenerator(
            ILLMService llMService,
            IConversionLogService conversionLogService,
            int conversionInstanceId,
            IDynamicPromptService dynamicPromptService
        ) : base(llMService, conversionLogService, conversionInstanceId)
        {
            DynamicPromptService = dynamicPromptService;
        }

        protected override List<TemplateFilesSummaryModel> GenerateSummaryModel(string promptResponse, TemplateFilesSplitModel splittedCodeModel)
        {
            return GetIterativeModelFromResponse(promptResponse);
        }

        protected override async Task<LLMPrompt> GetSummaryGenerationPrompt(TemplateFilesSplitModel splittedCodeModel, List<TemplateFilesSummaryModel> summaryJsonModel)
        {
            return await DynamicPromptService.GetDynamicPrompt(
                "templateIterativeJSONSummaryPrompt",
                new Dictionary<string, string>
                {
                    { "currentJsonToUpdate", JsonConvert.SerializeObject(new { templateSummaries = summaryJsonModel }, Formatting.Indented) },
                    { "templateFileName" , splittedCodeModel.fileName },
                    { "fileContent", splittedCodeModel.fileContent }
                },
                new Dictionary<string, string>()
                {

                }
            );
        }
        
        private List<TemplateFilesSummaryModel> GetIterativeModelFromResponse(string promptResponse) {
            var stringToWorkOn = promptResponse;
            int firstInstance = stringToWorkOn.IndexOf("```json");
            if (firstInstance == -1) {
                return JsonConvert.DeserializeObject<CombinedSummaryModel>(promptResponse).templateSummaries;
            }
            if (firstInstance != -1) {
                stringToWorkOn = stringToWorkOn.Substring(firstInstance + 7);
            } else {
                firstInstance = stringToWorkOn.IndexOf("```");
                stringToWorkOn = stringToWorkOn.Substring(firstInstance + 3);
            }

            firstInstance = stringToWorkOn.IndexOf("```");
            stringToWorkOn = stringToWorkOn.Substring(0, firstInstance);
            stringToWorkOn = stringToWorkOn.Replace("\"\"\"", "\"");
            
            return JsonConvert.DeserializeObject<CombinedSummaryModel>(stringToWorkOn).templateSummaries;
        }
    }
}