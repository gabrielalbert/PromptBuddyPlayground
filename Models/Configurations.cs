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
        public const string ADD_LANGUAGE=" using {0}";        
        public const string ADD_PROG_LANG = "Programming Language={0} ";
        public const string ADD_GENERATE_CODE = "Command={0} ";
        public const string ADD_REFERENCE_CODE = "Code='{0}'";
        public const string ADD_DEFAULT_UNIT_TEST = "Command= provide unit test cases for the code snippet ";
        public const string NO_TEXT_FOUND = "No text found";

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
        public const string OTHER = "";
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