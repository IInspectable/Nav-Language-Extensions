using System.Linq;
using System.Collections.Generic;

namespace Pharmatechnik.Nav.Language.Analyzer {

    public class Nav0111ChoiceNode0HasNoIncomingEdges: ITaskDefinitionAnalyzer {

        public DiagnosticDescriptor Descriptor => DiagnosticDescriptors.Semantic.Nav0111ChoiceNode0HasNoIncomingEdges;

        public IEnumerable<Diagnostic> Analyze(ITaskDefinitionSymbol taskDefinition, AnalyzerContext context) {
            //==============================
            //  Choice Node Errors
            //==============================
            foreach (var choiceNode in taskDefinition.NodeDeclarations.OfType<IChoiceNodeSymbol>()) {

                if (choiceNode.Outgoings.Any() && !choiceNode.Incomings.Any()) {

                    yield return new Diagnostic(
                        choiceNode.Location,
                        DiagnosticDescriptors.Semantic.Nav0111ChoiceNode0HasNoIncomingEdges,
                        choiceNode.Name);
                }
            }

        }

    }

}