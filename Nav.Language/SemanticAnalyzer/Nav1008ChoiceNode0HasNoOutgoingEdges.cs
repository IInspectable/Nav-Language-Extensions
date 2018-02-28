using System.Linq;
using System.Collections.Generic;

namespace Pharmatechnik.Nav.Language.SemanticAnalyzer {

    public class Nav1008ChoiceNode0HasNoOutgoingEdges: ITaskDefinitionAnalyzer {

        public DiagnosticDescriptor Descriptor => DiagnosticDescriptors.DeadCode.Nav1008ChoiceNode0HasNoOutgoingEdges;

        public IEnumerable<Diagnostic> Analyze(ITaskDefinitionSymbol taskDefinition, AnalyzerContext context) {
            //==============================
            //  The choice node '{0}' has no outgoing edges
            //==============================
            foreach (var choiceNode in taskDefinition.NodeDeclarations.OfType<IChoiceNodeSymbol>()) {

                if (choiceNode.Incomings.Any() && !choiceNode.Outgoings.Any()) {

                    yield return new Diagnostic(
                        choiceNode.Incomings.First().Location,
                        choiceNode.Incomings.Select(edge => edge.Location).Skip(1),
                        Descriptor,
                        choiceNode.Name);
                }
            }

        }

    }

}