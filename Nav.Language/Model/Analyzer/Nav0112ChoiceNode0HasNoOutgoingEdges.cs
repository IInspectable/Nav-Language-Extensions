using System.Linq;
using System.Collections.Generic;

namespace Pharmatechnik.Nav.Language.Analyzer {

    public class Nav0112ChoiceNode0HasNoOutgoingEdges: ITaskDefinitionAnalyzer {

        public DiagnosticDescriptor Descriptor => DiagnosticDescriptors.Semantic.Nav0112ChoiceNode0HasNoOutgoingEdges;

        public IEnumerable<Diagnostic> Analyze(ITaskDefinitionSymbol taskDefinition, AnalyzerContext context) {
            //==============================
            //  Choice Node Errors
            //==============================
            foreach (var choiceNode in taskDefinition.NodeDeclarations.OfType<IChoiceNodeSymbol>()) {

                if (choiceNode.Incomings.Any() && !choiceNode.Outgoings.Any()) {

                    yield return new Diagnostic(
                        choiceNode.Location,
                        Descriptor,
                        choiceNode.Name);
                }
            }

        }

    }

}