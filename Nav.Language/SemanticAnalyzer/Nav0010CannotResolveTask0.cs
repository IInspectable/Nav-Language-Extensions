using System.Linq;
using System.Collections.Generic;

namespace Pharmatechnik.Nav.Language.SemanticAnalyzer {

    public class Nav0010CannotResolveTask0: ITaskDefinitionAnalyzer {

        public DiagnosticDescriptor Descriptor => DiagnosticDescriptors.Semantic.Nav0010CannotResolveTask0;

        public IEnumerable<Diagnostic> Analyze(ITaskDefinitionSymbol taskDefinition, AnalyzerContext context) {
            //==============================
            // Cannot resolve task '{0}'
            //==============================
            foreach (var taskNode in taskDefinition.NodeDeclarations.OfType<ITaskNodeSymbol>()) {
                if (taskNode.Declaration == null) {

                    yield return (new Diagnostic(
                        taskNode.Location,
                        Descriptor,
                        taskNode.Name));
                }
            }

            // Bei einer Exit-Transition muss der Knoten vor dem Verbindungspunkt ein Task sein
            foreach (var exitTransition in taskDefinition.ExitTransitions) {

                if (exitTransition.TaskNodeSourceReference != null && exitTransition.TaskNodeSourceReference.Declaration == null) {

                    yield return new Diagnostic(
                        exitTransition.TaskNodeSourceReference.Location,
                        Descriptor,
                        exitTransition.TaskNodeSourceReference.Name);

                }
            }
        }

    }

}