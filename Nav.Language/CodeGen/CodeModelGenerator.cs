#region Using Directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Immutable;
using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language.CodeGen {

    public class CodeModelGenerator : Generator {

        public CodeModelGenerator(GenerationOptions options, PathProviderFactory pathProviderFactory=null) : base(options) {
            PathProviderFactory = pathProviderFactory ?? PathProviderFactory.Default;
        }

        [NotNull]
        public PathProviderFactory PathProviderFactory { get; }

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
            var pathProvider    = PathProviderFactory.CreatePathProvider(taskDefinition);

            IEnumerable<TOCodeModel> toCodeModels = null;
            if(Options.GenerateTOClasses) {
                toCodeModels = TOCodeModel.FromTaskDefinition(taskDefinition, pathProvider);
            }
            
            var codeModelResult = new CodeModelResult(
                taskDefinition     : taskDefinition,
                pathProvider       : pathProvider,
                beginWfsCodeModel  : IBeginWfsCodeModel.FromTaskDefinition(taskDefinition , pathProvider),
                iwfsCodeModel       : IWfsCodeModel.FromTaskDefinition(taskDefinition      , pathProvider),
                wfsBaseCodeModel   : WfsBaseCodeModel.FromTaskDefinition(taskDefinition   , pathProvider),
                wfsCodeModel: WfsCodeModel.FromTaskDefinition(taskDefinition, pathProvider),
                toCodeModels       : toCodeModels
            );
           
            return codeModelResult;
        }
    }
}