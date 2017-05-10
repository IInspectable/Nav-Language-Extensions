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
            var model   = new BeginWfsCodeModel(taskDefinition);
            var group   = new TemplateGroupString(Resources.BeginWFSTemplate);

            // IBegin...WFS
            var st = group.GetInstanceOf("IBeginWFS");
            st.Add("model", model);
            st.Add("context", context);

            var beginWfsInterfaceCode = st.Render();

            return new CodeGenerationResult(
                taskDefinition       : taskDefinition, 
                beginWfsInterfaceCode: beginWfsInterfaceCode);
        }
    }
}