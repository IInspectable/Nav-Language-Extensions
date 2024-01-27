using System.Linq;
using System.Collections.Generic;

namespace Pharmatechnik.Nav.Language.SemanticAnalyzer;

public class Nav0222Node0IsReachableByDifferentEdgeModes: NavAnalyzer {

    public override DiagnosticDescriptor Descriptor => DiagnosticDescriptors.Semantic.Nav0222Node0IsReachableByDifferentEdgeModes;

    public override IEnumerable<Diagnostic> Analyze(ITaskDefinitionSymbol taskDefinition, AnalyzerContext context) {

        // TODO Inits und Trigger betrachten

        //==============================
        // Node {0} is reachable by different edge modes
        //==============================
        foreach (IEdge edge in taskDefinition.Edges()) {

            foreach (var nodeCalls in edge.GetReachableCalls().GroupBy(c => c.Node)) {

                if (nodeCalls.GroupBy(c => c.EdgeMode.EdgeMode).Count() > 1) {

                    yield return new Diagnostic(
                        nodeCalls.First().EdgeMode.Location,
                        nodeCalls.Skip(1).Select(call => call.EdgeMode.Location),
                        Descriptor,
                        nodeCalls.Key.Name
                    );
                }
            }

            // Continuations
            foreach (var continuationCalls in edge.GetReachableContinuationCalls().GroupBy(c => c.Node)) {

                if (continuationCalls.GroupBy(c => c.EdgeMode.EdgeMode).Count() > 1) {

                    yield return new Diagnostic(
                        continuationCalls.First().EdgeMode.Location,
                        continuationCalls.Skip(1).Select(call => call.EdgeMode.Location),
                        Descriptor,
                        continuationCalls.Key.Name
                    );
                }
            }

        }

        // Exit Transitions betrachten
        foreach (var taskNode in taskDefinition.NodeDeclarations.OfType<ITaskNodeSymbol>()) {

            foreach (var nodeCalls in taskNode.Outgoings
                                              .SelectMany(exitTransition => exitTransition.GetReachableCalls())
                                              .GroupBy(c => c.Node)) {

                if (nodeCalls.GroupBy(c => c.EdgeMode.EdgeMode).Count() > 1) {

                    yield return new Diagnostic(
                        nodeCalls.First().EdgeMode.Location,
                        nodeCalls.Skip(1).Select(call => call.EdgeMode.Location),
                        Descriptor,
                        nodeCalls.Key.Name
                    );
                }
            }

            // Continuations
            foreach (var continuationCalls in taskNode.Outgoings
                                                      .SelectMany(exitTransition => exitTransition.GetReachableContinuationCalls())
                                                      .GroupBy(c => c.Node)) {

                if (continuationCalls.GroupBy(c => c.EdgeMode.EdgeMode).Count() > 1) {

                    yield return new Diagnostic(
                        continuationCalls.First().EdgeMode.Location,
                        continuationCalls.Skip(1).Select(call => call.EdgeMode.Location),
                        Descriptor,
                        continuationCalls.Key.Name
                    );
                }
            }

        }

    }

}