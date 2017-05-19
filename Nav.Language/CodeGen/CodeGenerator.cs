#region Using Directives

using System;
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
                taskDefinition: codeModelResult.TaskDefinition,
                pathProvider  : codeModelResult.PathProvider,
                iBeginWfsCode : GenerateIBeginWfsCode(codeModelResult.IBeginWfsCodeModel, context),
                iWfsCode      : GenerateIWfsCode(codeModelResult.IWfsCodeModel          , context),
                wfsBaseCode   : GenerateWfsBaseCode(codeModelResult.WfsBaseCodeModel    , context),
                wfsCode       : GenerateWfsCode(codeModelResult.WfsBaseCodeModel        , context));

            return codeGenerationResult;
        }
        
        static CodeGenerationSpec GenerateIBeginWfsCode(IBeginWfsCodeModel model, CodeGeneratorContext context) {

            var template = LoadTemplate(Resources.IBeginWfsTemplate, model, context);                        
            var content  = template.Render();

            return new CodeGenerationSpec(content, model.FilePath);
        }

        static CodeGenerationSpec GenerateIWfsCode(IWfsCodeModel model, CodeGeneratorContext context) {

            var template = LoadTemplate(Resources.IWfsTemplate, model, context);            
            var content = template.Render();

            return new CodeGenerationSpec(content, model.FilePath);
        }

        static CodeGenerationSpec GenerateWfsBaseCode(WfsBaseCodeModel model, CodeGeneratorContext context) {

            var template = LoadTemplate(Resources.WfsBaseTemplate, model, context);
            var content = template.Render();

            return new CodeGenerationSpec(content, model.FilePath);
        }

        static CodeGenerationSpec GenerateWfsCode(WfsBaseCodeModel model, CodeGeneratorContext context) {

            var template = LoadTemplate(Resources.WFSOneShotTemplate, model, context);            
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