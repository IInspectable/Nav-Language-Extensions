#region Using Directives

using System;
using System.Linq;
using System.Collections.Immutable;

using Antlr4.StringTemplate;
using JetBrains.Annotations;

using Pharmatechnik.Nav.Language.CodeGen.Templates;

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen {

    public class CodeGenerator {

        const string TemplateName         = "Begin";
        const string ModelAttributeName   = "model";
        const string ContextAttributeName = "context";

        public CodeGenerator(CodeGenerationOptions options) {
            Options = options ?? throw new ArgumentNullException(nameof(options));
        }

        [NotNull]
        public CodeGenerationOptions Options { get; }

        public IImmutableList<CodeGenerationResult> Generate(CodeGenerationUnit codeGenerationUnit) {

            if (codeGenerationUnit.Syntax.SyntaxTree.Diagnostics.HasErrors()) {
                throw new ArgumentException("Syntax errors detected");
            }

            if (codeGenerationUnit.Diagnostics.HasErrors()) {
                throw new ArgumentException("Semantic errors detected");
            }

            return codeGenerationUnit.TaskDefinitions
                                     .Select(Generate)
                                     .ToImmutableList();
        }

        CodeGenerationResult Generate(ITaskDefinitionSymbol taskDefinition) {

            var context = new CodeGeneratorContext(this);

            return new CodeGenerationResult(
                taskDefinition        : taskDefinition, 
                iBeginWfsInterfaceCode: GenerateIBeginWfsInterface(taskDefinition, context),
                iWfsInterfaceCode     : GenerateIWfsInterface(taskDefinition, context),
                wfsBaseCode           : GenerateWfsBase(taskDefinition, context),
                wfsOneShotCode        : GenerateWfsOneShot(taskDefinition, context));
        }

        static string GenerateIBeginWfsInterface(ITaskDefinitionSymbol taskDefinition, CodeGeneratorContext context) {

            var model = IBeginWfsCodeModel.FromTaskDefinition(taskDefinition);
            var group = new TemplateGroupString(Resources.IBeginWfsTemplate);
            
            var st = group.GetInstanceOf(TemplateName);
            st.Add(ModelAttributeName  , model);
            st.Add(ContextAttributeName, context);

            var result = st.Render();

            return result;
        }

        static string GenerateIWfsInterface(ITaskDefinitionSymbol taskDefinition, CodeGeneratorContext context) {

            var model = IWfsCodeModel.FromTaskDefinition(taskDefinition);
            var group = new TemplateGroupString(Resources.IWfsTemplate);

            var st = group.GetInstanceOf(TemplateName);
            st.Add(ModelAttributeName  , model);
            st.Add(ContextAttributeName, context);

            var result = st.Render();

            return result;
        }

        static string GenerateWfsBase(ITaskDefinitionSymbol taskDefinition, CodeGeneratorContext context) {

            var model = WfsBaseCodeModel.FromTaskDefinition(taskDefinition);
            var group = new TemplateGroupString(Resources.WfsBaseTemplate);

            var st = group.GetInstanceOf(TemplateName);
            st.Add(ModelAttributeName  , model);
            st.Add(ContextAttributeName, context);

            var result = st.Render();

            return result;
        }

        static string GenerateWfsOneShot(ITaskDefinitionSymbol taskDefinition, CodeGeneratorContext context) {

            var model = WfsBaseCodeModel.FromTaskDefinition(taskDefinition);
            var group = new TemplateGroupString(Resources.WFSOneShotTemplate);

            var st = group.GetInstanceOf(TemplateName);
            st.Add(ModelAttributeName, model);
            st.Add(ContextAttributeName, context);

            var result = st.Render();

            return result;
        }        
    }
}