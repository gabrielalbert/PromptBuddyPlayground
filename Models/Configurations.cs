using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PromptEngineering.Models
{

    public static class Configurations
    {
        public const string BUGFIX = "check the bugs and provide the fix for ";
        public const string NUNIT_TEST = "create nunit test method for ";
        public const string ADD_LANGUAGE = " using {0}";
        public const string ADD_PROG_LANG = "Programming Language={0} ";
        public const string ADD_GENERATE_CODE = "Command={0} ";
        public const string ADD_REFERENCE_CODE = "Code='{0}'";
        public const string ADD_DEFAULT_UNIT_TEST = "Command= provide unit test cases for the code snippet ";
        
        public const string NO_TEXT_FOUND = "No text found";

        public const string OUTPUT_FOLDER = "Output";
        public const string OUTPUT_FULL_FILENAME = "{0}\\{1}.txt";
        public const string DATE_FORMAT = "yyyyMMddhhmmsss";
        public const string COPILOT_CLI_SUGGEST = "gh copilot suggest -t shell '{0} {1}' >{2}.txt";
        public const string COPILOT_CLI_EXPLAIN = "gh copilot explain '{0} {1}' >{2}.txt";
        public const string CMD = "cmd.exe";
        public const string PROXY = "set HTTPS_PROXY=http://proxy.tcs.com";
        public const string NOPROXY = "set HTTPS_PROXY=";
        public const string CLS = "cls";
        public const string COPILOT_CLI_RESPONSE_SANITIZE1 = "see https://gh.io/gh-copilot-transparency";
        public const string COPILOT_CLI_RESPONSE_SANITIZE2 = "? Select an option  [Use arrows to move, type to filter]";
        public const string COPILOT_COMMAND_DOCS = "docs ";
        public const string COPILOT_COMMAND_BUGFIX = "check the bugs and provide the fix for ";
        public const string COPILOT_COMMAND_NUNIT_TEST = "create nunit test method for ";

        public const string COPILOT_ERROR_REDULT = "Error: Post \"https://api.github.com/graphql\": proxyconnect tcp: dial tcp: lookup proxy.tcs.com: getaddrinfow: This is usually a temporary error during hostname resolution and means that the local server did not receive a response from an authoritative server.";

        public const string COPILOT_RESULT_RETRY = " not readily available. Please revise for better results.";

        public const string REPHARSE_PROMPT = "? What would you like the shell command to do? ";
        public const string COPILOT_NUNIT_EXPLAIN = "gh copilot explain {0} '{1}' >{2}.txt";
        public const string COPILOT_DOCS_EXPLAIN = "gh copilot explain {0} '{1}' >{2}.txt";

        public const string COPILOT_CLI_EXPLAIN_COMMAND = "gh copilot explain";
        public const string ADD_DEFAULT_DOCS = "Command=provide documentation for the code snippet ";
        public const string ADD_DEFAULT_XMLDOCS = "Command=provide xml documentation for the code snippet ";
        public const string ADD_DEFAULT_BUGFIX = "Command=check the bugs for the code snippet ";

    }

    public static class CLIPhase
    {
        public const string CODE = "code";
        public const string DOCS = "docs";
        public const string REPHRASE = "rephrase";
        public const string UNIT_TEST = "unittest";
        public const string BUG_FIX = "bugfix";
        public const string XMLDOCS = "xmldocs";
        public const string SECURITY_FIX = "securityfix";
        public const string EXPLAIN = "";
        public const string OTHER = "other";
        public const string CONVERT = "convert";
    }

    public static class PromptEnggType
    {
        public const string ZeroShot = "Zero-Shot";
        public const string OneShot = "One-Shot";
        public const string FewShot = "Few-Shot";
        public const string ChainofThought = "Chain-of-Thought Prompting";
        public const string Iterative = "Iterative Prompting";
        public const string Negative = "Negative Prompting";
        public const string Hybrid = "Hybrid Prompting";
        public const string PromptChaining = "Prompt Chaining";

    }
}