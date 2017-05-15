#region Using Directives

using System;
using System.Linq;
using System.Collections.Immutable;

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen {

    public class CodeModelGenerator : Generator {

        public CodeModelGenerator(GenerationOptions options) : base(options) {
        }

        public IImmutableList<CodeModelResult> Generate(CodeGenerationUnit codeGenerationUnit) {

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

        CodeModelResult Generate(ITaskDefinitionSymbol taskDefinition) {

            var codeModelResult = new CodeModelResult(
                taskDefinition   : taskDefinition,
                beginWfsCodeModel: IBeginWfsCodeModel.FromTaskDefinition(taskDefinition),
                wfsCodeModel     : IWfsCodeModel.FromTaskDefinition(taskDefinition),
                wfsBaseCodeModel : WfsBaseCodeModel.FromTaskDefinition(taskDefinition)
            );
           
            return codeModelResult;
        }
    }
}