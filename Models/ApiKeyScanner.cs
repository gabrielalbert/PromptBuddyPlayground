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
        // CREDIT_CARD
        new Regex(@"\b(?:\d[ -]*?){13,16}\b", RegexOptions.Compiled),

        // EMAIL
        new Regex(@"\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Z|a-z]{2,}\b", RegexOptions.Compiled),

        // PHONE
        new Regex(@"\b(?:\+\d{1,2}\s?)?(?:\(\d{3}\)|\d{3})[-\s]?\d{3}[-\s]?\d{4}\b", RegexOptions.Compiled),

        // SSN
        new Regex(@"\b\d{3}-\d{2}-\d{4}\b", RegexOptions.Compiled),

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
