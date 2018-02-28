using System.Collections.Generic;

namespace Pharmatechnik.Nav.Language.SemanticAnalyzer {

    public class Nav0012CannotResolveExit0: ITaskDefinitionAnalyzer {

        public DiagnosticDescriptor Descriptor => DiagnosticDescriptors.Semantic.Nav0012CannotResolveExit0;

        public IEnumerable<Diagnostic> Analyze(ITaskDefinitionSymbol taskDefinition, AnalyzerContext context) {
            //==============================
            // Cannot resolve exit '{0}'
            //==============================
            foreach (var exitTransition in taskDefinition.ExitTransitions) {

                if (exitTransition.ConnectionPointReference != null && exitTransition.ConnectionPointReference.Declaration == null) {
                    yield return new Diagnostic(
                        exitTransition.ConnectionPointReference.Location,
                        Descriptor,
                        exitTransition.ConnectionPointReference.Name);

                }
            }

        }

    }

}