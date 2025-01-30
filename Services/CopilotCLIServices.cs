using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PromptEngineering.Models;
using PromptEngineering.Repository;
using PromptEngineering.Utils;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace PromptEngineering.Services
{
    public class CopilotCLIServices:ICopilotCLIServices
    {
        private readonly IChatRepository _chatRepository;
        private readonly ILogger<CopilotCLIServices> _logger;
        private readonly IMapper _mapper;
        private readonly MySettings _settings;
        private string outputFolder = AppDomain.CurrentDomain.BaseDirectory + Configurations.OUTPUT_FOLDER;
        private TimeSpan delayBetweenAttempts;
        public CopilotCLIServices(ILogger<CopilotCLIServices> logger, IMapper mapper, IChatRepository chatRepository, IOptions<MySettings> settings)
        {
            _logger = logger;
            _mapper = mapper;
            _chatRepository = chatRepository;
            _settings = settings.Value;
            delayBetweenAttempts = TimeSpan.FromSeconds(Convert.ToDouble(_settings.DelayBetweenAttemptInSeconds));
        }

        public async Task<Chats> ExecuteCopilotCommand(Chats chats, bool proxyEnabled = false)
        {
            
            try
            {               
                
                chats.RequestedTime = DateTime.Now;
                chats.Status = "In Progress";

                chats = ZeroShotPrompt(chats);
                if (chats.Success == false)
                {
                    chats = OneShotPrompt(chats);
                }

                if (chats.Success == false)
                {
                    chats = await IterativePrompt(chats);
                }

                //result.MessageText = chats.Result.ReplaceEscapeChars();
                //result.MessageSender = MessageSender.Bot;
                //result.Feedback = string.Empty;
                //result.MessageDate = chats.RespondedTime;
                //result.PromptEnggType = chats.PromptEnggType;
                //var chatId = _chatRepository.AddChats(chats);
                _logger.LogInformation($"Command executed successfully with output: {chats.Result}");
                //result.ChatId = chatId;
                //result.MessageId = chatId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                //result.ChatId = -1;
                //result.MessageId = 0;
                //result.MessageText = "An error occurred";
                //result.MessageSender = MessageSender.Bot;
                //result.Feedback = string.Empty;

            }
            return chats;

        }
        private Chats ZeroShotPrompt(Chats message,bool proxyEnabled=false)
        {

            try
            {
                string output;
                string error;
                string copilotCommand = GetCopilotCommand(message.Phase, message.Prompt, message.Reference, message.Language);
                _logger.LogInformation($"ZeroShotPrompt Copilot Command: {copilotCommand}");
                ExecuteCopilotCliCommand(copilotCommand, proxyEnabled, out output, out error);

                _logger.LogInformation($"ZeroShotPrompt output string {output}");
                message.Attempt = 0;
                message.RespondedTime = DateTime.Now;
                message.Status = "Completed";
                message.PromptEnggType = PromptEnggType.ZeroShot;
                if (string.IsNullOrEmpty(output) || output.Contains(Configurations.COPILOT_RESULT_RETRY, StringComparison.OrdinalIgnoreCase) || (!string.IsNullOrEmpty(error)))
                {
                    message.Success = false;
                    message.Result = (!string.IsNullOrEmpty(error) ? error : output);
                }
                else
                {
                    message.Result = output;
                    message.Success = true;
                }

            }
            catch (Exception ex)
            {
                _logger.LogInformation($"ZeroShotPrompt failed: {ex.Message}");
            }

            return message;
        }

        private Chats OneShotPrompt(Chats message, bool proxyEnabled = false)
        {

            try
            {
                string output;
                string error;
                string copilotCommand = GetCopilotCommand(message.Phase, message.Prompt, message.Reference, message.Language);
                _logger.LogInformation($"OneShotPrompt Copilot Command: {copilotCommand}");
                ExecuteCopilotCliCommand(copilotCommand, proxyEnabled, out output, out error);

                if (!string.IsNullOrEmpty(error) || output.Contains(Configurations.COPILOT_RESULT_RETRY, StringComparison.OrdinalIgnoreCase))
                {
                    copilotCommand = GetCopilotCommand(message.Phase, message.Prompt, message.Reference, message.Language, true);
                    ExecuteCopilotCliCommand(copilotCommand, proxyEnabled, out output, out error);
                }

                _logger.LogInformation($"OneShotPrompt output string {output}");
                message.Attempt = 1;
                message.Result = output;
                message.RespondedTime = DateTime.Now;
                message.Status = "Completed";
                message.PromptEnggType = PromptEnggType.OneShot;
                message.Success = true;

            }
            catch (Exception ex)
            {
                _logger.LogInformation($"OneShotPrompt failed: {ex.Message}");
            }

            return message;
        }

        private async Task<Chats> IterativePrompt(Chats message, bool proxyEnabled = false)
        {

            try
            {
                string output = string.Empty;
                string error = string.Empty;
                int attempt = 1;

                while (attempt <= Convert.ToInt32(_settings.MaxAttempts))
                {
                    string copilotCommand = GetCopilotCommand(message.Phase, message.Prompt, message.Reference, message.Language);
                    _logger.LogInformation($"IterativePrompt Copilot Attempt {attempt} Command: {copilotCommand}");
                    ExecuteCopilotCliCommand(copilotCommand, proxyEnabled, out output, out error);

                    if (!string.IsNullOrEmpty(output) && output.Contains(Configurations.COPILOT_RESULT_RETRY, StringComparison.OrdinalIgnoreCase))
                    {
                        attempt++;
                        await Task.Delay(delayBetweenAttempts);
                    }
                    else
                    {
                        break;
                    }

                }
                if (!string.IsNullOrEmpty(output) && output.Contains(Configurations.COPILOT_RESULT_RETRY, StringComparison.OrdinalIgnoreCase))
                {
                    message.Success = false;
                }
                else
                {
                    message.Success = true;

                }
                string prefixResultContent = (output.IndexOf(Configurations.COPILOT_CLI_RESPONSE_SANITIZE1) > 0 ? output.Substring(0, output.IndexOf(Configurations.COPILOT_CLI_RESPONSE_SANITIZE1) + 44) : string.Empty);
                string postfixResultContent = (output.LastIndexOf(Configurations.COPILOT_CLI_RESPONSE_SANITIZE2) > 0 ? output.Substring(output.LastIndexOf(Configurations.COPILOT_CLI_RESPONSE_SANITIZE2)) : string.Empty);
                output = (string.IsNullOrEmpty(prefixResultContent) ? output : output.Replace(prefixResultContent, string.Empty));
                output = (string.IsNullOrEmpty(postfixResultContent) ? output : output.Replace(postfixResultContent, string.Empty));
                _logger.LogInformation($"IterativePrompt output string {output}");

                message.Result = output;
                message.RespondedTime = DateTime.Now;
                message.Status = "Completed";
                message.PromptEnggType = PromptEnggType.Iterative;

            }
            catch (Exception ex)
            {
                _logger.LogInformation($"IterativePrompt failed: {ex.Message}");
            }

            return message;
        }
        private void ExecuteCopilotCliCommand(string command, bool proxyEnabled, out string output, out string error)
        {
            if (proxyEnabled)
            {
                ExecuteCopilotCliCommandWithProxy(command, out output, out error);
            }
            else
            {
                ExecuteCopilotCliCommandWithNoProxy(command, out output, out error);
            }
        }


        private void ExecuteCopilotCliCommandWithProxy(string command, out string output, out string error)
        {
            Process process = null;

            try
            {
                string outputFilename = DateTime.Now.ToString(Configurations.DATE_FORMAT) + Guid.NewGuid();
                string combainedCommand = $"{command}>{outputFilename}.txt";
                process = DefaultProcessArguments(Configurations.PROXY);
                process.Start();

                process.StandardInput.WriteLine(combainedCommand);
                _logger.LogInformation("CLI Command Executed");

                // Read the output from the command
                string tempOutput = process.StandardOutput.ReadToEnd();
                string outputFilePath = String.Format(Configurations.OUTPUT_FULL_FILENAME, outputFolder, outputFilename);
                output = System.IO.File.ReadAllText(outputFilePath, Encoding.UTF8);
                _logger.LogInformation($"CLI output string {output}");
                error = process.StandardError.ReadToEnd();

                output = output.CleanCliOutput();
            }
            finally
            {
                process.WaitForExit();
                process.Kill();
                process.Close();
            }
        }
        private void ExecuteCopilotCliCommandWithNoProxy(string command, out string output, out string error)
        {
            Process process = null;
            output=string.Empty;
            error = string.Empty;
            try
            {
                process = new Process();


                //process.StartInfo.FileName = Configurations.CMD;               

                process.StartInfo.FileName = "/bin/bash";
                process.StartInfo.WorkingDirectory = outputFolder;
                process.StartInfo.Arguments = "-c \" " + command + " \"";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.CreateNoWindow = true;

                //process = DefaultProcessArguments(command);
                process.Start();

                _logger.LogInformation("CLI Command Executed");

                // Read the output from the command
                output = process.StandardOutput.ReadToEnd();
                _logger.LogInformation($"CLI output string {output}");
                error = process.StandardError.ReadToEnd();
                output = output.CleanCliOutput();

            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                //throw ex;
            }           
            finally
            {
                process.WaitForExit();
                process.Kill();
                process.Close();
            }
        }

        private Process DefaultProcessArguments(string command)
        {
            Process process = new Process();
            //process.StartInfo.FileName = "/bin/bash";// Configurations.CMD;
            //process.StartInfo.WorkingDirectory = outputFolder;
            process.StartInfo.Arguments = $"/C {command}";
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            return process;
        }

        private string GetCopilotCommand(string phase, string command, string referenceCode, string language = "", bool repharse = false)
        {
            _logger.LogInformation($"GetCopilotCommand method started at {DateTime.Now}");
            _logger.LogInformation($"GetCopilotCommand method Input {phase} {command} {referenceCode}");
            try
            {
                command = command.ReplaceNewLineToSpaceChars();
                referenceCode = referenceCode.ReplaceNewLineToSpaceChars();
                string copilotCommand = string.Empty;
                if (language.Equals("General", StringComparison.OrdinalIgnoreCase) && phase.Equals(CLIPhase.OTHER, StringComparison.OrdinalIgnoreCase))
                {
                    copilotCommand = $"{Configurations.COPILOT_CLI_EXPLAIN_COMMAND} {(repharse ? CLIPhase.REPHRASE : string.Empty)} {command} {(string.IsNullOrEmpty(referenceCode) ? string.Empty : "'" + referenceCode.ReplaceNewLineChars() + "'")}";
                }
                else if (language.Equals("General", StringComparison.OrdinalIgnoreCase) && phase.Equals(CLIPhase.CODE, StringComparison.OrdinalIgnoreCase))
                {
                    copilotCommand = $"{Configurations.COPILOT_CLI_EXPLAIN_COMMAND} {(repharse ? CLIPhase.REPHRASE : string.Empty)} {CLIPhase.CODE} \"{command} {(string.IsNullOrEmpty(referenceCode) ? string.Empty : "'" + referenceCode.ReplaceNewLineChars() + "'")}\"";
                }
                else if (language.Equals("General", StringComparison.OrdinalIgnoreCase) && phase.Equals(CLIPhase.UNIT_TEST, StringComparison.OrdinalIgnoreCase))
                {
                    copilotCommand = $"{Configurations.COPILOT_CLI_EXPLAIN_COMMAND} {(repharse ? CLIPhase.REPHRASE : string.Empty)} {CLIPhase.UNIT_TEST} \"{command} {(string.IsNullOrEmpty(referenceCode) ? string.Empty : "'" + referenceCode.ReplaceNewLineChars() + "'")}\"";
                }
                else if (language.Equals("General", StringComparison.OrdinalIgnoreCase) && phase.Equals(CLIPhase.BUG_FIX, StringComparison.OrdinalIgnoreCase))
                {
                    copilotCommand = $"{Configurations.COPILOT_CLI_EXPLAIN_COMMAND} {(repharse ? CLIPhase.REPHRASE : string.Empty)} {CLIPhase.BUG_FIX} \"{command} {(string.IsNullOrEmpty(referenceCode) ? string.Empty : "'" + referenceCode.ReplaceNewLineChars() + "'")}\"";
                }
                else if (language.Equals("General", StringComparison.OrdinalIgnoreCase) && phase.Equals(CLIPhase.XMLDOCS, StringComparison.OrdinalIgnoreCase))
                {
                    copilotCommand = $"{Configurations.COPILOT_CLI_EXPLAIN_COMMAND} {(repharse ? CLIPhase.REPHRASE : string.Empty)} {CLIPhase.XMLDOCS} \"{command} {(string.IsNullOrEmpty(referenceCode) ? string.Empty : "'" + referenceCode.ReplaceNewLineChars() + "'")}\"";
                }
                else if (language.Equals("General", StringComparison.OrdinalIgnoreCase) && phase.Equals(CLIPhase.DOCS, StringComparison.OrdinalIgnoreCase))
                {
                    copilotCommand = $"{Configurations.COPILOT_CLI_EXPLAIN_COMMAND} {(repharse ? CLIPhase.REPHRASE : string.Empty)} {CLIPhase.DOCS} \"{command} {(string.IsNullOrEmpty(referenceCode) ? string.Empty : "'" + referenceCode.ReplaceNewLineChars() + "'")}\"";
                }
                else if (language.Equals("General", StringComparison.OrdinalIgnoreCase) && phase.Equals(CLIPhase.SECURITY_FIX, StringComparison.OrdinalIgnoreCase))
                {
                    copilotCommand = $"{Configurations.COPILOT_CLI_EXPLAIN_COMMAND} {(repharse ? CLIPhase.REPHRASE : string.Empty)} {CLIPhase.SECURITY_FIX} \"{command} {(string.IsNullOrEmpty(referenceCode) ? string.Empty : "'" + referenceCode.ReplaceNewLineChars() + "'")}\"";
                }
                else if (language.Equals("General", StringComparison.OrdinalIgnoreCase) && phase.Equals(CLIPhase.EXPLAIN, StringComparison.OrdinalIgnoreCase))
                {
                    copilotCommand = $"{Configurations.COPILOT_CLI_EXPLAIN_COMMAND} {(repharse ? CLIPhase.REPHRASE : string.Empty)} \"{command} {(string.IsNullOrEmpty(referenceCode) ? string.Empty : "'" + referenceCode.ReplaceNewLineChars() + "'")}\"";
                }
                else if (!language.Equals("General", StringComparison.OrdinalIgnoreCase) && phase.Equals(CLIPhase.CODE, StringComparison.OrdinalIgnoreCase))
                {
                    string addLang = (command.Contains(language, StringComparison.OrdinalIgnoreCase) ? string.Empty : string.Format(Configurations.ADD_PROG_LANG, language));
                    string addCommand = (string.IsNullOrEmpty(command) ? string.Empty : string.Format(Configurations.ADD_GENERATE_CODE, command));
                    string addCode = (string.IsNullOrEmpty(referenceCode) ? string.Empty : string.Format(Configurations.ADD_REFERENCE_CODE, referenceCode.ReplaceNewLineChars()));
                    copilotCommand = $"{Configurations.COPILOT_CLI_EXPLAIN_COMMAND} {CLIPhase.CODE} \"{addLang} {addCommand} {addCode}\"";
                    _logger.LogInformation($"CLI command with lang added {copilotCommand}");
                }
                else if (!language.Equals("General", StringComparison.OrdinalIgnoreCase) && phase.Equals(CLIPhase.UNIT_TEST, StringComparison.OrdinalIgnoreCase))
                {
                    //string addLang = (command.Contains(language, StringComparison.OrdinalIgnoreCase) ? string.Empty : string.Format(Configurations.ADD_PROG_LANG, language));
                    string addCommand = (string.IsNullOrEmpty(command) ? Configurations.ADD_DEFAULT_UNIT_TEST : string.Format(Configurations.ADD_GENERATE_CODE, command));
                    string addCode = (string.IsNullOrEmpty(referenceCode) ? string.Empty : string.Format(Configurations.ADD_REFERENCE_CODE, referenceCode.ReplaceNewLineChars()));
                    copilotCommand = $"{Configurations.COPILOT_CLI_EXPLAIN_COMMAND} {CLIPhase.UNIT_TEST} \"{addCommand} {addCode}\"";
                    _logger.LogInformation($"CLI command with lang added {copilotCommand}");
                }
                else if (!language.Equals("General", StringComparison.OrdinalIgnoreCase) && phase.Equals(CLIPhase.BUG_FIX, StringComparison.OrdinalIgnoreCase))
                {
                    string addLang = (command.Contains(language, StringComparison.OrdinalIgnoreCase) ? string.Empty : string.Format(Configurations.ADD_PROG_LANG, language));
                    string addCommand = (string.IsNullOrEmpty(command) ? Configurations.ADD_DEFAULT_BUGFIX : string.Format(Configurations.ADD_GENERATE_CODE, command));
                    string addCode = (string.IsNullOrEmpty(referenceCode) ? string.Empty : string.Format(Configurations.ADD_REFERENCE_CODE, referenceCode.ReplaceNewLineChars()));
                    copilotCommand = $"{Configurations.COPILOT_CLI_EXPLAIN_COMMAND} {CLIPhase.BUG_FIX} \"{addLang} {addCommand} {addCode} \"";
                    _logger.LogInformation($"CLI command with lang added {copilotCommand}");
                }
                else if (!language.Equals("General", StringComparison.OrdinalIgnoreCase) && phase.Equals(CLIPhase.DOCS, StringComparison.OrdinalIgnoreCase))
                {
                    string addLang = (command.Contains(language, StringComparison.OrdinalIgnoreCase) ? string.Empty : string.Format(Configurations.ADD_PROG_LANG, language));
                    string addCommand = (string.IsNullOrEmpty(command) ? Configurations.ADD_DEFAULT_DOCS : string.Format(Configurations.ADD_GENERATE_CODE, command));
                    string addCode = (string.IsNullOrEmpty(referenceCode) ? string.Empty : string.Format(Configurations.ADD_REFERENCE_CODE, referenceCode.ReplaceNewLineChars()));
                    copilotCommand = $"{Configurations.COPILOT_CLI_EXPLAIN_COMMAND} \"{addLang} {addCommand} {addCode} \"";
                    _logger.LogInformation($"CLI command with lang added {copilotCommand}");
                }
                else if (!language.Equals("General", StringComparison.OrdinalIgnoreCase) && phase.Equals(CLIPhase.XMLDOCS, StringComparison.OrdinalIgnoreCase))
                {
                    string addLang = (command.Contains(language, StringComparison.OrdinalIgnoreCase) ? string.Empty : string.Format(Configurations.ADD_PROG_LANG, language));
                    string addCommand = (string.IsNullOrEmpty(command) ? Configurations.ADD_DEFAULT_XMLDOCS : string.Format(Configurations.ADD_GENERATE_CODE, command));
                    string addCode = (string.IsNullOrEmpty(referenceCode) ? string.Empty : string.Format(Configurations.ADD_REFERENCE_CODE, referenceCode.ReplaceNewLineChars()));
                    copilotCommand = $"{Configurations.COPILOT_CLI_EXPLAIN_COMMAND} \"{addLang} {addCommand} {addCode} \"";
                    _logger.LogInformation($"CLI command with lang added {copilotCommand}");
                }

                _logger.LogInformation($"CLI command framed {copilotCommand}");
                return copilotCommand;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw ex;
            }

        }
    }
}
