using System.Linq;
using System.Collections.Generic;

namespace Pharmatechnik.Nav.Language.SemanticAnalyzer;

public class Nav0222Node0IsReachableByDifferentEdgeModes: NavAnalyzer {

    public override DiagnosticDescriptor Descriptor => DiagnosticDescriptors.Semantic.Nav0222Node0IsReachableByDifferentEdgeModes;

    public override IEnumerable<Diagnostic> Analyze(ITaskDefinitionSymbol taskDefinition, AnalyzerContext context) {

        //==============================
        // Node {0} is reachable by different edge modes
        //==============================

        // Init Transitions betrachten
        foreach (var initNodeSymbol in taskDefinition.NodeDeclarations.OfType<IInitNodeSymbol>()) {
            // die "normalen inits" erlauben sowieso nur  --> (Nav0110)
         
            // Continuations
            foreach (var diagnostic in Analyze(initNodeSymbol.Outgoings.SelectMany(exitTransition => exitTransition.GetReachableContinuations()))) {
                yield return diagnostic;
            }
        }

        // Trigger Transitions betrachten
        foreach (var triggerTransition in taskDefinition.Edges().OfType<ITriggerTransition>()) {
            foreach (var diagnostic in Analyze(triggerTransition.AsEnumerable())) {
                yield return diagnostic;
            }

            // Continuations
            foreach (var diagnostic in Analyze(triggerTransition.GetReachableContinuations())) {
                yield return diagnostic;
            }
        }

        // Exit Transitions betrachten
        foreach (var taskNode in taskDefinition.NodeDeclarations.OfType<ITaskNodeSymbol>()) {

            foreach (var diagnostic in Analyze(taskNode.Outgoings)) {
                yield return diagnostic;
            }

            // Continuations
            foreach (var diagnostic in Analyze(taskNode.Outgoings.SelectMany(exitTransition => exitTransition.GetReachableContinuations()))) {
                yield return diagnostic;
            }
        }

    }

    IEnumerable<Diagnostic> Analyze(IEnumerable<IEdge> edges) {

        foreach (var nodeCalls in edges.SelectMany(edge => edge.GetReachableCalls())
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
    }

}