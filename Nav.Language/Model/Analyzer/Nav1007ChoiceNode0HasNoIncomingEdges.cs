using System.Linq;
using System.Collections.Generic;

namespace Pharmatechnik.Nav.Language.Analyzer {

    public class Nav1007ChoiceNode0HasNoIncomingEdges: ITaskDefinitionAnalyzer {

        public DiagnosticDescriptor Descriptor => DiagnosticDescriptors.DeadCode.Nav1007ChoiceNode0HasNoIncomingEdges;

        public IEnumerable<Diagnostic> Analyze(ITaskDefinitionSymbol taskDefinition, AnalyzerContext context) {
            //==============================
            //  Choice Node Errors
            //==============================
            foreach (var choiceNode in taskDefinition.NodeDeclarations.OfType<IChoiceNodeSymbol>()) {

                if (choiceNode.Outgoings.Any() && !choiceNode.Incomings.Any()) {

                    yield return new Diagnostic(
                        choiceNode.Outgoings.First().Location,
                        choiceNode.Outgoings.Select(edge => edge.Location).Skip(1),
                        Descriptor,
                        choiceNode.Name);
                }
            }

        }

    }

}