using System.Linq;
using System.Collections.Generic;

namespace Pharmatechnik.Nav.Language.SemanticAnalyzer {

    public class Nav0109InitNode0HasNoOutgoingEdges: ITaskDefinitionAnalyzer {

        public DiagnosticDescriptor Descriptor => DiagnosticDescriptors.Semantic.Nav0109InitNode0HasNoOutgoingEdges;

        public IEnumerable<Diagnostic> Analyze(ITaskDefinitionSymbol taskDefinition, AnalyzerContext context) {
            //==============================
            //  The init node '{0}' has no outgoing edges
            //==============================
            foreach (var initNode in taskDefinition.NodeDeclarations.OfType<IInitNodeSymbol>()) {

                if (!initNode.Outgoings.Any()) {

                    yield return new Diagnostic(
                        initNode.Alias?.Location ?? initNode.Location,
                        Descriptor,
                        initNode.Name);
                }
            }

        }

    }

}