using System.Linq;
using System.Collections.Generic;

namespace Pharmatechnik.Nav.Language.Analyzer {

    public class Nav0025NoOutgoingEdgeForExit0Declared: ITaskDefinitionAnalyzer {

        public DiagnosticDescriptor Descriptor => DiagnosticDescriptors.Semantic.Nav0025NoOutgoingEdgeForExit0Declared;

        public IEnumerable<Diagnostic> Analyze(ITaskDefinitionSymbol taskDefinition, AnalyzerContext context) {
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