using System.Collections.Generic;

namespace Pharmatechnik.Nav.Language.SemanticAnalyzer {

    public class Nav0106EndNode0MustOnlyReachedByGoTo: ITaskDefinitionAnalyzer {

        public DiagnosticDescriptor Descriptor => DiagnosticDescriptors.Semantic.Nav0106EndNode0MustOnlyReachedByGoTo;

        public IEnumerable<Diagnostic> Analyze(ITaskDefinitionSymbol taskDefinition, AnalyzerContext context) {
            //==============================
            // End node '{0}' must only be reached by -->
            //==============================
            foreach (var transition in taskDefinition.Edges()) {

                if (transition.TargetReference?.Declaration is EndNodeSymbol &&
                    transition.EdgeMode          != null                     &&
                    transition.EdgeMode.EdgeMode != EdgeMode.Goto) {

                    yield return (new Diagnostic(
                        transition.EdgeMode.Location,
                        Descriptor,
                        transition.TargetReference.Name));
                }
            }
        }

    }

}