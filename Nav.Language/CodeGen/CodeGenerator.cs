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

            var codeModelResult = new CodeModelResult(
                taskDefinition   : taskDefinition,
                beginWfsCodeModel: IBeginWfsCodeModel.FromTaskDefinition(taskDefinition),
                wfsCodeModel     : IWfsCodeModel.FromTaskDefinition(taskDefinition),
                wfsBaseCodeModel : WfsBaseCodeModel.FromTaskDefinition(taskDefinition)
                );

            var codeGenerationResult= new CodeGenerationResult(
                taskDefinition        : taskDefinition,
                iBeginWfsInterfaceCode: GenerateIBeginWfsInterface(codeModelResult.IBeginWfsCodeModel, context),
                iWfsInterfaceCode     : GenerateIWfsInterface(codeModelResult.IWfsCodeModel          , context),
                wfsBaseCode           : GenerateWfsBase(codeModelResult.WfsBaseCodeModel             , context),
                wfsOneShotCode        : GenerateWfsOneShot(codeModelResult.WfsBaseCodeModel          , context));

            return codeGenerationResult;
        }

        static string GenerateIBeginWfsInterface(IBeginWfsCodeModel model, CodeGeneratorContext context) {

            var group = new TemplateGroupString(Resources.IBeginWfsTemplate);            
            var st    = group.GetInstanceOf(TemplateName);

            st.Add(ModelAttributeName  , model);
            st.Add(ContextAttributeName, context);

            var result = st.Render();

            return result;
        }

        static string GenerateIWfsInterface(IWfsCodeModel model, CodeGeneratorContext context) {

            var group = new TemplateGroupString(Resources.IWfsTemplate);
            var st    = group.GetInstanceOf(TemplateName);

            st.Add(ModelAttributeName  , model);
            st.Add(ContextAttributeName, context);

            var result = st.Render();

            return result;
        }

        static string GenerateWfsBase(WfsBaseCodeModel model, CodeGeneratorContext context) {

            var group = new TemplateGroupString(Resources.WfsBaseTemplate);
            var st    = group.GetInstanceOf(TemplateName);

            st.Add(ModelAttributeName  , model);
            st.Add(ContextAttributeName, context);

            var result = st.Render();

            return result;
        }

        static string GenerateWfsOneShot(WfsBaseCodeModel model, CodeGeneratorContext context) {

            var group = new TemplateGroupString(Resources.WFSOneShotTemplate);
            var st    = group.GetInstanceOf(TemplateName);

            st.Add(ModelAttributeName, model);
            st.Add(ContextAttributeName, context);

            var result = st.Render();

            return result;
        }        
    }
}