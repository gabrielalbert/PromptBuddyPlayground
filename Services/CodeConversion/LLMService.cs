using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Routing;
using PromptEngineering.Models.DynamicPrompt;
using PromptEngineering.Repository;

namespace PromptEngineering.Services.CodeConversion
{
public class LLMService : ILLMService
    {
        public class OllamaJSONOptionsModel
        {
            public double? temperature { get; set; }
            public int? seed { get; set; }
        }

        public class OllamaJSONModel
        {
            public string model { get; set; }
            public string prompt { get; set; }
            public string system { get; set; }
            public bool stream { get; set; }
            public List<int> context { get; set; }
            public OllamaJSONOptionsModel options { get; set; }

        }

        public class OllamaResponseModel
        {
            public string response { get; set; }
            public List<int> context { get; set; }
            public long total_duration { get; set; }
        }

        private HttpClient httpClient;
        private readonly IChatRepository _chatRepository;

        public LLMService(IChatRepository chatRepository)
        {
            _chatRepository = chatRepository;
            httpClient = new HttpClient();
            httpClient.Timeout = new TimeSpan(6, 0, 0);
        }

        public async Task<PromptTestResponse> GetResponseDirect(LLMPrompt promptInput)
        {
            var result = _chatRepository.GetAiEndpointUrl("ollama", promptInput.ModelName);
            var uri = new Uri(result.endpointUrl + "api/generate");

            var response = await httpClient.PostAsJsonAsync(uri, GetOllamaJSONModel(promptInput));
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return new PromptTestResponse { Error = true };
            }
            var ollamaResponse = await response.Content.ReadFromJsonAsync<OllamaResponseModel>();
            return new PromptTestResponse
            {
                Error = false,
                Response = ollamaResponse.response,
                Context = ollamaResponse.context,
                Duration = (int)Math.Floor((double)ollamaResponse.total_duration/1000)
            };
        }

        private OllamaJSONModel GetOllamaJSONModel(LLMPrompt llMPrompt)
        {
            return new OllamaJSONModel
            {
                model = llMPrompt.ModelName,
                prompt = llMPrompt.UserPrompt,
                system = llMPrompt.SystemPrompt,
                stream = false,
                context = new List<int>(),
                options = new OllamaJSONOptionsModel
                {
                    temperature = llMPrompt.Temperature,
                    seed = llMPrompt.Seed
                }
            };
        }
    }
}