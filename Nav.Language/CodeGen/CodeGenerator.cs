#region Using Directives

using System;
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using System.Collections.Immutable;

using Antlr4.StringTemplate;
using JetBrains.Annotations;

using Pharmatechnik.Nav.Language.CodeGen.Templates;
// ReSharper disable InconsistentNaming

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen {

    public class CodeGenerator: Generator {

        const string TemplateName         = "Begin";
        const string ModelAttributeName   = "model";
        const string ContextAttributeName = "context";

        public CodeGenerator(GenerationOptions options, PathProviderFactory pathProviderFactory = null) : base(options) {
            PathProviderFactory = pathProviderFactory ?? PathProviderFactory.Default;
        }

        [NotNull]
        public PathProviderFactory PathProviderFactory { get; }

        public IImmutableList<CodeGenerationResult> Generate(CodeGenerationUnit codeGenerationUnit) {

            if (codeGenerationUnit == null) {
                throw new ArgumentNullException(nameof(codeGenerationUnit));
            }
            if (codeGenerationUnit.Syntax.SyntaxTree.Diagnostics.HasErrors()) {
                throw new ArgumentException("Syntax errors detected");
            }
            if (codeGenerationUnit.Diagnostics.HasErrors()) {
                throw new ArgumentException("Semantic errors detected");
            }

            var codeModelResult = codeGenerationUnit.TaskDefinitions
                                                    .Select(GenerateCodeModel)
                                                    .ToImmutableList();

            return codeModelResult.Select(GenerateCode).ToImmutableList();
        }

        CodeModelResult GenerateCodeModel(ITaskDefinitionSymbol taskDefinition) {
            var pathProvider = PathProviderFactory.CreatePathProvider(taskDefinition);

            IEnumerable<TOCodeModel> toCodeModels = null;
            if (Options.GenerateTOClasses) {
                toCodeModels = TOCodeModel.FromTaskDefinition(taskDefinition, pathProvider);
            }

            var codeModelResult = new CodeModelResult(
                taskDefinition   : taskDefinition,
                pathProvider     : pathProvider,
                beginWfsCodeModel: IBeginWfsCodeModel.FromTaskDefinition(taskDefinition, pathProvider),
                iwfsCodeModel    : IWfsCodeModel.FromTaskDefinition(taskDefinition, pathProvider),
                wfsBaseCodeModel : WfsBaseCodeModel.FromTaskDefinition(taskDefinition, pathProvider),
                wfsCodeModel     : WfsCodeModel.FromTaskDefinition(taskDefinition, pathProvider),
                toCodeModels     : toCodeModels
            );

            return codeModelResult;
        }

        CodeGenerationResult GenerateCode(CodeModelResult codeModelResult) {
            var context = new CodeGeneratorContext(this);

            var codeGenerationResult = new CodeGenerationResult(
                taskDefinition   : codeModelResult.TaskDefinition,
                pathProvider     : codeModelResult.PathProvider,
                iBeginWfsCodeSpec: GenerateIBeginWfsCodeSpec(codeModelResult.IBeginWfsCodeModel, context),
                iWfsCodeSpec     : GenerateIWfsCodeSpec(codeModelResult.IWfsCodeModel, context),
                wfsBaseCodeSpec  : GenerateWfsBaseCodeSpec(codeModelResult.WfsBaseCodeModel, context),
                wfsCodeSpec      : GenerateWfsCodeSpec(codeModelResult.WfsCodeModel, context),
                toCodeSpecs      : GenerateToCodeSpecs(codeModelResult.TOCodeModels, context));

            return codeGenerationResult;
        }
        
        static readonly ThreadLocal<TemplateGroup> IBeginWfsTemplateGroup= new ThreadLocal<TemplateGroup>(() => LoadTemplateGroup(Resources.IBeginWfsTemplate));

        static CodeGenerationSpec GenerateIBeginWfsCodeSpec(IBeginWfsCodeModel model, CodeGeneratorContext context) {

            var template = GetTemplate(IBeginWfsTemplateGroup.Value, model, context);
            var content  = template.Render();

            return new CodeGenerationSpec(content, model.FilePath);
        }

        static readonly ThreadLocal<TemplateGroup> IWfsTemplateGroup = new ThreadLocal<TemplateGroup>(() => LoadTemplateGroup(Resources.IWfsTemplate));

        static CodeGenerationSpec GenerateIWfsCodeSpec(IWfsCodeModel model, CodeGeneratorContext context) {

            var template = GetTemplate(IWfsTemplateGroup.Value, model, context);
            var content = template.Render();

            return new CodeGenerationSpec(content, model.FilePath);
        }

        static readonly ThreadLocal<TemplateGroup> WfsBaseTemplateGroup = new ThreadLocal<TemplateGroup>(() => LoadTemplateGroup(Resources.WfsBaseTemplate));

        static CodeGenerationSpec GenerateWfsBaseCodeSpec(WfsBaseCodeModel model, CodeGeneratorContext context) {

            var template = GetTemplate(WfsBaseTemplateGroup.Value, model, context);
            var content = template.Render();

            return new CodeGenerationSpec(content, model.FilePath);
        }

        static readonly ThreadLocal<TemplateGroup> WfsTemplateGroup = new ThreadLocal<TemplateGroup>(() => LoadTemplateGroup(Resources.WFSOneShotTemplate));
        
        static CodeGenerationSpec GenerateWfsCodeSpec(WfsCodeModel model, CodeGeneratorContext context) {

            var template = GetTemplate(WfsTemplateGroup.Value, model, context);
            var content  = template.Render();

            return new CodeGenerationSpec(content, model.FilePath);
        }

        static IEnumerable<CodeGenerationSpec> GenerateToCodeSpecs(IEnumerable<TOCodeModel> models, CodeGeneratorContext context) {
            return models.Select(model => GenerateToCodeSpec(model, context));
        }

        static readonly ThreadLocal<TemplateGroup> ToTemplateGroup = new ThreadLocal<TemplateGroup>(() => LoadTemplateGroup(Resources.TOTemplate));
        
        static CodeGenerationSpec GenerateToCodeSpec(TOCodeModel model, CodeGeneratorContext context) {

            var template = GetTemplate(ToTemplateGroup.Value, model, context);
            var content  = template.Render();

            return new CodeGenerationSpec(content, model.FilePath);
        }

        static TemplateGroup LoadTemplateGroup(string resourceName) {

            var codeGenFacts   = new TemplateGroupString(Resources.CodeGenFacts);
            var commonTemplate = new TemplateGroupString(Resources.CommonTemplate);
            var templateGroup  = new TemplateGroupString(resourceName);

            templateGroup.ImportTemplates(codeGenFacts);
            templateGroup.ImportTemplates(commonTemplate);

            return templateGroup;
        }

        static Template GetTemplate(TemplateGroup templateGroup, CodeModel model, CodeGeneratorContext context) {
            var st = templateGroup.GetInstanceOf(TemplateName);

            st.Add(ModelAttributeName  , model);
            st.Add(ContextAttributeName, context);

            return st;
        }
    }
}