using System.Linq;
using System.Collections.Generic;

namespace Pharmatechnik.Nav.Language.SemanticAnalyzer {

    public class Nav0025NoOutgoingEdgeForExit0Declared: NavAnalyzer {

        public override DiagnosticDescriptor Descriptor => DiagnosticDescriptors.Semantic.Nav0025NoOutgoingEdgeForExit0Declared;

        public override IEnumerable<Diagnostic> Analyze(ITaskDefinitionSymbol taskDefinition, AnalyzerContext context) {
            //==============================
            //  No outgoing edge for exit '{0}' declared
            //==============================
            foreach (var taskNode in taskDefinition.NodeDeclarations.OfType<ITaskNodeSymbol>()) {

                if (taskNode.References.Any() && taskNode.Declaration != null) {

                    var expectedExits = taskNode.Declaration.Exits().OrderBy(cp => cp.Name);
                    var actualExits = taskNode.Outgoings
                                              .Select(et => et.ConnectionPointReference)
                                              .Where(cp => cp != null)
                                              .ToList();

                    foreach (var expectedExit in expectedExits) {

                        if (!actualExits.Exists(cpRef => cpRef.Declaration == expectedExit)) {

                            yield return new Diagnostic(
                                taskNode.Location,
                                taskNode.Incomings
                                        .Select(edge => edge.TargetReference)
                                        .Where(nodeReference => nodeReference != null)
                                        .Select(nodeReference => nodeReference.Location),
                                Descriptor,
                                expectedExit.Name);
                        }
                    }

                }
            }
        }

    }

}