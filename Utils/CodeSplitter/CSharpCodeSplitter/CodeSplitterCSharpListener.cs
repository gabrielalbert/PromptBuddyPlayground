using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Antlr4.Runtime.Misc;

namespace PromptEngineering.Utils.CodeSplitter.CSharpCodeSplitter
{
    public class CodeSplitterCSharpListener : CSharpParserBaseListener
    {
        private CSharpCodeSplitterProcessor CodeSplitterProcessor;
        public CodeSplitterCSharpListener(CSharpCodeSplitterProcessor codeSplitterProcessor) {
            CodeSplitterProcessor = codeSplitterProcessor;
        }

        public override void EnterUsing_directives([NotNull] CSharpParser.Using_directivesContext context)
        {
            CodeSplitterProcessor.InjestStart((int)CSChunkType.USINGS_DIRECTIVE, context.Start.StartIndex);
            base.EnterUsing_directives(context);
        }

        public override void ExitUsing_directives([NotNull] CSharpParser.Using_directivesContext context)
        {
            CodeSplitterProcessor.InjestEnd((int)CSChunkType.USINGS_DIRECTIVE, context.Start.StartIndex, context.Stop.StopIndex);
            base.ExitUsing_directives(context);
        }

        public override void EnterNamespace_declaration([NotNull] CSharpParser.Namespace_declarationContext context)
        {
            CodeSplitterProcessor.InjestStart((int)CSChunkType.NAMESPACE_DECLARATION, context.Start.StartIndex);
            base.EnterNamespace_declaration(context);
        }

        public override void ExitNamespace_declaration([NotNull] CSharpParser.Namespace_declarationContext context)
        {
            CodeSplitterProcessor.InjestEnd((int)CSChunkType.NAMESPACE_DECLARATION, context.Start.StartIndex, context.Stop.StopIndex);
            base.ExitNamespace_declaration(context);
        }

        public override void EnterConstructor_declaration([NotNull] CSharpParser.Constructor_declarationContext context)
        {
            CodeSplitterProcessor.InjestStart((int)CSChunkType.CONSTRUCTOR_DECLARATION, context.Start.StartIndex);
            base.EnterConstructor_declaration(context);
        }

        public override void ExitConstructor_declaration([NotNull] CSharpParser.Constructor_declarationContext context)
        {
            CodeSplitterProcessor.InjestEnd((int)CSChunkType.CONSTRUCTOR_DECLARATION, context.Start.StartIndex, context.Stop.StopIndex);
            base.ExitConstructor_declaration(context);
        }

        public override void EnterTyped_member_declaration([NotNull] CSharpParser.Typed_member_declarationContext context)
        {
            CodeSplitterProcessor.InjestStart((int)CSChunkType.TYPED_MEMBER_DECLARATION, context.Start.StartIndex);
            base.EnterTyped_member_declaration(context);
        }

        public override void ExitTyped_member_declaration([NotNull] CSharpParser.Typed_member_declarationContext context)
        {
            CodeSplitterProcessor.InjestEnd((int)CSChunkType.TYPED_MEMBER_DECLARATION, context.Start.StartIndex, context.Stop.StopIndex);
            base.ExitTyped_member_declaration(context);
        }

        public override void EnterMethod_declaration([NotNull] CSharpParser.Method_declarationContext context)
        {
            CodeSplitterProcessor.InjestStart((int)CSChunkType.METHOD_DECLARATION, context.Start.StartIndex);
            base.EnterMethod_declaration(context);
        }

        public override void ExitMethod_declaration([NotNull] CSharpParser.Method_declarationContext context)
        {
            CodeSplitterProcessor.InjestEnd((int)CSChunkType.METHOD_DECLARATION, context.Start.StartIndex, context.Stop.StopIndex);
            base.ExitMethod_declaration(context);
        }

        public override void EnterClass_definition([NotNull] CSharpParser.Class_definitionContext context)
        {   
            CodeSplitterProcessor.InjestStart((int)CSChunkType.CLASS_DEFINITION, context.Start.StartIndex);
            base.EnterClass_definition(context);
        }

        public override void ExitClass_definition([NotNull] CSharpParser.Class_definitionContext context)
        {
            CodeSplitterProcessor.InjestEnd((int)CSChunkType.CLASS_DEFINITION, context.Start.StartIndex, context.Stop.StopIndex);
            base.ExitClass_definition(context);
        }
    }
}