using System.Collections.Generic;

using Pharmatechnik.Nav.Language.CodeGen;

namespace Pharmatechnik.Nav.Language.SemanticAnalyzer {

    public class Nav2000IdentifierExpected: ITaskDefinitionAnalyzer {

        public DiagnosticDescriptor Descriptor => DiagnosticDescriptors.Semantic.Nav2000IdentifierExpected;

        public IEnumerable<Diagnostic> Analyze(ITaskDefinitionSymbol taskDefinition, AnalyzerContext context) {
            //==============================
            // Identifier expected
            //==============================
            if (!CSharp.IsValidIdentifier(taskDefinition.Name)) {
                yield return new Diagnostic(
                    taskDefinition.Location,
                    DiagnosticDescriptors.Semantic.Nav2000IdentifierExpected);
            }
        }

    }

}