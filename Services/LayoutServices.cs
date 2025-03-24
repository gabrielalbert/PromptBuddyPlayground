using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PromptEngineering.Models;
using PromptEngineering.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OpenAI;
using OpenAI.Chat;
using System.Threading;


namespace PromptEngineering.Services
{
    public class LayoutServices: ILayoutServices
    {
        private readonly ILogger<LayoutServices> _logger;
        private readonly string _fileStoragePath;
        private readonly ILayoutRepository _layoutRepository;
        private readonly MySettings _settings;
        

        public LayoutServices(ILogger<LayoutServices> logger,ILayoutRepository layoutRepository, IOptions<MySettings> settings)
        {
            _fileStoragePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UploadedFiles");
            if (!Directory.Exists(_fileStoragePath))
            {
                Directory.CreateDirectory(_fileStoragePath);
            }
            _logger=logger;
            _layoutRepository= layoutRepository;
            _settings=(MySettings)settings.Value;            
        }
        private readonly string[] _supportedFileTypes = new[]{
            ".png",".jpeg",".jpg"
        };

        public async Task<string> AddUiLayoutEntry(LayoutModel input)
        {
            try
            {
                var fileExtension = Path.GetExtension(input.ImageFileName).ToLower();
                if (!_supportedFileTypes.Contains(fileExtension))
                {
                    return "Error:File type not supported";
                }

                var uniqueFileName = await UploadFile(input.ImageFileName, input.ImageFile);
                if (!string.IsNullOrEmpty(uniqueFileName))
                {
                    input.FileName = uniqueFileName;
                    input.UploadedOn = DateTime.Now;
                    input.ProcessedBy = input.UploadedBy;
                    input.UploadedOn = DateTime.Now;
                    input.Status = "uploaded";

                    // Save the file details to the database
                    var uiLayoutId=await _layoutRepository.AddUiLayoutEntry(input);
                    return $"Success:`{uiLayoutId}`";
                }
                else
                {
                    return "Error:Unable to save the image file";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file");
                return "Error:Error uploading file";
            }           

           
        }
        public async Task<List<LayoutModel>> GetAllUiLayoutDatas()
        {
            try
            {
                return await _layoutRepository.GetAllUiLayoutDatas();                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting layout data");
                return new List<LayoutModel>();
            }
        }

        public async Task<(string,string)> DownloadFile(int layoutId)
        {
            try
            {
                var layoutData = await _layoutRepository.GetUiLayoutDatas(layoutId);
                if (layoutData == null)
                {
                    return (null, "");
                }

                return (layoutData.ResponseMessage, layoutData.OutputFileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading file");
                return (null, "");
            }
        }

        public async Task<string> GetImageContent(int layoutId)
        {
            try
            {
                var layoutData = await _layoutRepository.GetUiLayoutDatas(layoutId);
                if (layoutData == null)
                {
                    return (null);
                }
                var imageFileName = layoutData.FileName;
                return await ReadImageFile(imageFileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading file");
                return (null);
            }
        }

        public async Task<string> StartProcess(int layoutId, string userName)
        {
            try
            {
                // Step 1: Add task entry to DB with 'inprogress' status
                await _layoutRepository.UpdateUiLayoutEntryStatus(layoutId, "inprogress", userName);

                //await RunBackgroundTaskAsync(layoutId, userName);
                // Step 2: Trigger the background task
                Task.Run(()=> RunBackgroundTaskAsync(layoutId, userName));
                return "success";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting process");
                return "error";
            }
        }
        // Background task that runs the triggered API call
        private async Task RunBackgroundTaskAsync(int layoutId, string userName)
        {                        
            try
            {
                var layoutData =  await _layoutRepository.GetUiLayoutDatas(layoutId);

                var credential = new System.ClientModel.ApiKeyCredential("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");

                var openAIOptions = new OpenAIClientOptions()
                {
                    Endpoint = new System.Uri(_settings.ImageToCodeUrl)
                };

                var client = new ChatClient(_settings.LocalModel4UiLayout, credential, openAIOptions);//GitHub AI        

                var imagePath = Path.Combine(_fileStoragePath, layoutData.FileName);

                string prompt = @$"Please analyze the image and recreate a design in {layoutData.Frontend} frontend application with {layoutData.CssFramework} framework.
Please generate the complete code and styles for this structure, ensuring it is visually appealing, responsive, and follows best practices.
{(string.IsNullOrEmpty(layoutData.CustomizePrompt) ? string.Empty : layoutData.CustomizePrompt)}";

                // Convert image to Base64
                byte[] imageBytes = await File.ReadAllBytesAsync(imagePath);
                
                var contentParts = new List<ChatMessageContentPart>();
                contentParts.Add(ChatMessageContentPart.CreateImagePart(new BinaryData(imageBytes), "image/png", default(ChatImageDetailLevel)));
                contentParts.Add(ChatMessageContentPart.CreateTextPart(prompt));
                

                // Create message with image
                var uMessage = new UserChatMessage(contentParts.AsEnumerable());

                var message = new List<OpenAI.Chat.ChatMessage>();
                message.Add(uMessage);

                var chatcompletions = new ChatCompletionOptions()
                {
                    Temperature = (float)0.5,
                    TopP = 1,
                    FrequencyPenalty = 0,
                    PresencePenalty = 0,
                };

                string finalResult = "";
                await foreach (var text in client.CompleteChatStreamingAsync(message.AsEnumerable(), chatcompletions, CancellationToken.None))
                {
                    finalResult+= text.ContentUpdate.Count>0 ? text.ContentUpdate.FirstOrDefault().Text : string.Empty;                    
                }

                //// Step 1: Add task entry to DB with 'inprogress' status
                await _layoutRepository.UpdateResult(layoutId, finalResult, userName);

            }
            catch (Exception ex)
            {
                // Handle exceptions, set status to 'Failed'
                await _layoutRepository.UpdateUiLayoutEntryStatus(layoutId, "failed", userName);
            }
        }
        private async Task<string> UploadFile(string filename,string filecontent)
        {
            try
            {
                var fileExtension = Path.GetExtension(filename).ToLower();
                var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";

                // Decode Base64 string to byte array
                byte[] imageBytes = Convert.FromBase64String(filecontent);

                // Save or process the image bytes as needed
                var filePath = Path.Combine(_fileStoragePath, uniqueFileName);
                await System.IO.File.WriteAllBytesAsync(filePath, imageBytes);

                return uniqueFileName;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file");
                return null;
            }
            
        }

        private async Task<string> ReadImageFile(string filename)
        {
            try
            {
                
                var filePath = Path.Combine(_fileStoragePath, filename);
                /// Read the image file into a byte array
                byte[] imageBytes =await File.ReadAllBytesAsync(filePath);

                // Convert the byte array to a Base64 string
                string base64String = Convert.ToBase64String(imageBytes);

                return base64String;
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file");
                return null;
            }

        }
    }
}
