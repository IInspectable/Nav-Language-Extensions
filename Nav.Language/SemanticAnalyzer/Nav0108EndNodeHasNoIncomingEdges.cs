using System.Linq;
using System.Collections.Generic;

namespace Pharmatechnik.Nav.Language.SemanticAnalyzer {

    public class Nav0108EndNodeHasNoIncomingEdges: ITaskDefinitionAnalyzer {

        public DiagnosticDescriptor Descriptor => DiagnosticDescriptors.Semantic.Nav0108EndNodeHasNoIncomingEdges;

        public IEnumerable<Diagnostic> Analyze(ITaskDefinitionSymbol taskDefinition, AnalyzerContext context) {
            //==============================
            //  The end node has no incoming edges
            //==============================
            foreach (var endNode in taskDefinition.NodeDeclarations.OfType<IEndNodeSymbol>()) {
               
                if (!endNode.Incomings.Any()) {

                    yield return new Diagnostic(
                        endNode.Location,
                        Descriptor,
                        endNode.Name);
                }
            }
        }

    }

}