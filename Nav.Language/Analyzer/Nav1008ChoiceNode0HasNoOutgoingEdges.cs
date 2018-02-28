using System.Linq;
using System.Collections.Generic;

namespace Pharmatechnik.Nav.Language.Analyzer {

    public class Nav1008ChoiceNode0HasNoOutgoingEdges: ITaskDefinitionAnalyzer {

        public DiagnosticDescriptor Descriptor => DiagnosticDescriptors.DeadCode.Nav1008ChoiceNode0HasNoOutgoingEdges;

        public IEnumerable<Diagnostic> Analyze(ITaskDefinitionSymbol taskDefinition, AnalyzerContext context) {
            //==============================
            //  Choice Node Errors
            //==============================
            foreach (var choiceNode in taskDefinition.NodeDeclarations.OfType<IChoiceNodeSymbol>()) {

                if (choiceNode.Incomings.Any() && !choiceNode.Outgoings.Any()) {

                    if (choiceNode.Incomings.Any()) {
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

}