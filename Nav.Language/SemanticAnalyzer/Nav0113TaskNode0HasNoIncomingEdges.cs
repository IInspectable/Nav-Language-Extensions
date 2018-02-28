using System.Linq;
using System.Collections.Generic;

namespace Pharmatechnik.Nav.Language.SemanticAnalyzer {

    public class Nav0113TaskNode0HasNoIncomingEdges: ITaskDefinitionAnalyzer {

        public DiagnosticDescriptor Descriptor => DiagnosticDescriptors.Semantic.Nav0113TaskNode0HasNoIncomingEdges;

        public IEnumerable<Diagnostic> Analyze(ITaskDefinitionSymbol taskDefinition, AnalyzerContext context) {
            foreach (var taskNode in taskDefinition.NodeDeclarations.OfType<ITaskNodeSymbol>()) {

                if (!taskNode.References.Any()) {

                } else {

                    if (!taskNode.Incomings.Any()) {

                        yield return new Diagnostic(
                            taskNode.Location,
                            Descriptor,
                            taskNode.Name);

                    }
                }

            }
        }

    }

}