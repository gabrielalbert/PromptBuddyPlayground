using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PromptEngineering.Models
{

    public static class ApiKeyScanner
    {
        // Add more patterns as needed
        private static readonly List<Regex> ApiKeyPatterns = new()
    {
        // Generic API Key (32+ alphanumeric)
        new Regex(@"\b[A-Za-z0-9]{32,}\b", RegexOptions.Compiled),

        // Common providers
        new Regex(@"sk-[A-Za-z0-9]{32,}", RegexOptions.Compiled), // OpenAI
        new Regex(@"AIza[0-9A-Za-z\-_]{35}", RegexOptions.Compiled), // Google
        new Regex(@"ghp_[A-Za-z0-9]{36}", RegexOptions.Compiled), // GitHub
        new Regex(@"Bearer\s+[A-Za-z0-9\-_\.]+", RegexOptions.Compiled), // Bearer token
    };

        public static bool ContainsApiKey(string input)
        {
            return ApiKeyPatterns.Any(pattern => pattern.IsMatch(input));
        }

        public static IEnumerable<string> ExtractApiKeys(string input)
        {
            return ApiKeyPatterns.SelectMany(pattern => pattern.Matches(input).Select(m => m.Value));
        }
    }

}
