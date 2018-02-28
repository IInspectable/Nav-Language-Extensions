using System.Linq;
using System.Collections.Generic;

namespace Pharmatechnik.Nav.Language.SemanticAnalyzer {

    public class Nav1010TaskNode0HasNoIncomingEdges: ITaskDefinitionAnalyzer {

        public DiagnosticDescriptor Descriptor => DiagnosticDescriptors.DeadCode.Nav1010TaskNode0HasNoIncomingEdges;

        public IEnumerable<Diagnostic> Analyze(ITaskDefinitionSymbol taskDefinition, AnalyzerContext context) {
            foreach (var taskNode in taskDefinition.NodeDeclarations.OfType<ITaskNodeSymbol>()) {

                if (!taskNode.References.Any()) {

                } else {

                    if (!taskNode.Incomings.Any()) {

                        if (taskNode.Outgoings.Any()) {
                            yield return new Diagnostic(
                                taskNode.Outgoings.First().Location,
                                taskNode.Outgoings.Select(edge => edge.Location).Skip(1),
                                Descriptor,
                                taskNode.Name);
                        }
                    }
                }

            }
        }

    }

}