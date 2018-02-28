using System.Linq;
using System.Collections.Generic;

namespace Pharmatechnik.Nav.Language.Analyzer {

    public class Nav0107ExitNode0HasNoIncomingEdges: ITaskDefinitionAnalyzer {

        public DiagnosticDescriptor Descriptor => DiagnosticDescriptors.Semantic.Nav0107ExitNode0HasNoIncomingEdges;

        public IEnumerable<Diagnostic> Analyze(ITaskDefinitionSymbol taskDefinition, AnalyzerContext context) {
            //==============================
            //  Exit Node Errors
            //==============================
            foreach (var exitNode in taskDefinition.NodeDeclarations.OfType<IExitNodeSymbol>()) {
                if (!exitNode.Incomings.Any()) {

                    yield return new Diagnostic(
                        exitNode.Location,
                        Descriptor,
                        exitNode.Name);
                }
            }

        }

    }

}