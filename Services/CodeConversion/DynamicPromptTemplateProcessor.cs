using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PromptEngineering.Services.CodeConversion
{
    public static class DynamicPromptTemplateProcessor
    {
        private static string GetKeyPlaceHolder(string key)
        {
            return "{{" + key + "}}";
        }

        public static List<string> GetArrayFromCommaSeperatedKeys(string commaSeperatedKeys)
        {
            commaSeperatedKeys = commaSeperatedKeys.Trim();
            return string.IsNullOrEmpty(commaSeperatedKeys) ? new List<string>() : commaSeperatedKeys.Split(",").ToList();
        }

        public static string GetPromptStringFromTemplate(string promptTemplate, Dictionary<string, string> templateKeyValues)
        {
            foreach (var templateKeyValue in templateKeyValues)
            {
                promptTemplate = promptTemplate.Replace(GetKeyPlaceHolder(templateKeyValue.Key), templateKeyValue.Value);
            }
            return promptTemplate;
        }

        public static (bool, List<string>) IsPromptTemplateStringValid(string promptTemplate, List<string> templateKeys)
        {
            var errorMessages = new List<string>();
            var actualKeyPlaceHolders = templateKeys.Select(t => GetKeyPlaceHolder(t));

            var regex = new Regex("{{[a-zA-Z0-9]+}}");
            var templatePlaceHolders = regex.Matches(promptTemplate).Select(entry => entry.ToString());

            var templateMissingPlaceHolders = actualKeyPlaceHolders.Where(e => !templatePlaceHolders.Contains(e));
            var templateExtraPlaceholders = templatePlaceHolders.Where(e => !actualKeyPlaceHolders.Contains(e));

            if (templateMissingPlaceHolders.Count() > 0)
            {
                foreach (var missingPlaceHolder in templateMissingPlaceHolders)
                {
                    errorMessages.Add($"Missing key in template: {missingPlaceHolder}");
                }
            }
            if (templateExtraPlaceholders.Count() > 0)
            {
                foreach (var extraPlaceHolder in templateExtraPlaceholders)
                {
                    errorMessages.Add($"Unknown key in template: {extraPlaceHolder}");
                }
            }

            if (errorMessages.Count != 0)
            {
                return (false, errorMessages);
            }
            return (true, new List<string>());
        }
    }
}