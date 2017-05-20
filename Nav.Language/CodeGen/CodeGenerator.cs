#region Using Directives

using System;
using System.Collections.Generic;
using System.Linq;
using Antlr4.StringTemplate;
using Pharmatechnik.Nav.Language.CodeGen.Templates;

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen {

    public class CodeGenerator: Generator {

        const string TemplateName         = "Begin";
        const string ModelAttributeName   = "model";
        const string ContextAttributeName = "context";

        public CodeGenerator(GenerationOptions options): base(options) {
        }

        public CodeGenerationResult Generate(CodeModelResult codeModelResult) {

            if (codeModelResult == null) {
                throw new ArgumentNullException(nameof(codeModelResult));
            }

            var context = new CodeGeneratorContext(this);

            var codeGenerationResult = new CodeGenerationResult(
                taskDefinition    : codeModelResult.TaskDefinition,
                pathProvider      : codeModelResult.PathProvider,
                iBeginWfsCodeSpec : GenerateIBeginWfsCodeSpec(codeModelResult.IBeginWfsCodeModel, context),
                iWfsCodeSpec      : GenerateIWfsCodeSpec(codeModelResult.IWfsCodeModel          , context),
                wfsBaseCodeSpec   : GenerateWfsBaseCodeSpec(codeModelResult.WfsBaseCodeModel    , context),
                wfsCodeSpec       : GenerateWfsCodeSpec(codeModelResult.WfsCodeModel            , context),
                toCodeSpecs       : GenerateToCodeSpecs(codeModelResult.TOCodeModels            , context));

            return codeGenerationResult;
        }
        
        static CodeGenerationSpec GenerateIBeginWfsCodeSpec(IBeginWfsCodeModel model, CodeGeneratorContext context) {

            var template = LoadTemplate(Resources.IBeginWfsTemplate, model, context);                        
            var content  = template.Render();

            return new CodeGenerationSpec(content, model.FilePath);
        }

        static CodeGenerationSpec GenerateIWfsCodeSpec(IWfsCodeModel model, CodeGeneratorContext context) {

            var template = LoadTemplate(Resources.IWfsTemplate, model, context);            
            var content = template.Render();

            return new CodeGenerationSpec(content, model.FilePath);
        }

        static CodeGenerationSpec GenerateWfsBaseCodeSpec(WfsBaseCodeModel model, CodeGeneratorContext context) {

            var template = LoadTemplate(Resources.WfsBaseTemplate, model, context);
            var content = template.Render();

            return new CodeGenerationSpec(content, model.FilePath);
        }

        static CodeGenerationSpec GenerateWfsCodeSpec(WfsCodeModel model, CodeGeneratorContext context) {

            var template = LoadTemplate(Resources.WFSOneShotTemplate, model, context);            
            var content  = template.Render();

            return new CodeGenerationSpec(content, model.FilePath);
        }

        static IEnumerable<CodeGenerationSpec> GenerateToCodeSpecs(IEnumerable<TOCodeModel> models, CodeGeneratorContext context) {
            return models.Select(model => GenerateToCodeSpec(model, context));
        }

        static CodeGenerationSpec GenerateToCodeSpec(TOCodeModel model, CodeGeneratorContext context) {

            var template = LoadTemplate(Resources.TOTemplate, model, context);
            var content  = template.Render();

            return new CodeGenerationSpec(content, model.FilePath);
        }

        static Template LoadTemplate(string resourceName, CodeModel model, CodeGeneratorContext context) {

            var commonTemplate = new TemplateGroupString(Resources.CommonTemplate);
            var templateGroup  = new TemplateGroupString(resourceName);

            templateGroup.ImportTemplates(commonTemplate);

            var st = templateGroup.GetInstanceOf(TemplateName);

            st.Add(ModelAttributeName, model);
            st.Add(ContextAttributeName, context);

            return st;
        }
    }
}