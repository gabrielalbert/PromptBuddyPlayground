using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PromptEngineering.Models.DynamicPrompt;
using PromptEngineering.Services.CodeConversion;
using PromptEngineering.Utils.CodeSplitter.CSharpCodeSplitter;

namespace PromptEngineering.Utils.SummmaryGenerators.NETSummaryGenerator
{
    public class NETSummaryGenerator : SummaryGeneratorBase<CSharpSplittedCodeModel, NETSummaryModel>
    {
        private IDynamicPromptService DynamicPromptService { get; set; }

        public NETSummaryGenerator(
            ILLMService llMService,
            IConversionLogService conversionLogService,
            int conversionInstanceId,
            IDynamicPromptService dynamicPromptService
        ) : base(llMService, conversionLogService, conversionInstanceId)
        {
            DynamicPromptService = dynamicPromptService;
        }

        protected override List<NETSummaryModel> GenerateSummaryModel(string promptResponse, CSharpSplittedCodeModel splittedCodeModel)
        {
            return GetIterativeModelFromResponse(promptResponse);
        }

        protected override async Task<LLMPrompt> GetSummaryGenerationPrompt(CSharpSplittedCodeModel splittedCodeModel, List<NETSummaryModel> summaryJsonModel)
        {
            return await DynamicPromptService.GetDynamicPrompt(
                "csharpIterativeJSONSummaryPrompt",
                new Dictionary<string, string>
                {
                    { "currentJsonToUpdate", JsonConvert.SerializeObject(new { methodSummaries = summaryJsonModel }, Formatting.Indented) },
                    { "className" , splittedCodeModel.ClassName },
                    { "methodContent", splittedCodeModel.MethodContent }
                },
                new Dictionary<string, string>()
                {
                    
                }
            );
        }
        
        private List<NETSummaryModel> GetIterativeModelFromResponse(string promptResponse) {
            var stringToWorkOn = promptResponse;
            int firstInstance = stringToWorkOn.IndexOf("```json");
            if (firstInstance == -1) {
                return JsonConvert.DeserializeObject<CombinedSummaryModel>(promptResponse).methodSummaries;
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
            
            return JsonConvert.DeserializeObject<CombinedSummaryModel>(stringToWorkOn).methodSummaries;
        }
    }
}