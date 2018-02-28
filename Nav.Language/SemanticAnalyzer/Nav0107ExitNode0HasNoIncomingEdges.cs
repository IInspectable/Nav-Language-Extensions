using System.Linq;
using System.Collections.Generic;

namespace Pharmatechnik.Nav.Language.SemanticAnalyzer {

    public class Nav0107ExitNode0HasNoIncomingEdges: ITaskDefinitionAnalyzer {

        public DiagnosticDescriptor Descriptor => DiagnosticDescriptors.Semantic.Nav0107ExitNode0HasNoIncomingEdges;

        public IEnumerable<Diagnostic> Analyze(ITaskDefinitionSymbol taskDefinition, AnalyzerContext context) {
            //==============================
            //  The exit node '{0}' has no incoming edges
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