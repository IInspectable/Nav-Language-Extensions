#nullable enable

using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Pharmatechnik.Nav.Language.SemanticAnalyzer;

public class Nav1022DifferentViewsInConcatNotSupported: NavAnalyzer {

    public override DiagnosticDescriptor Descriptor => DiagnosticDescriptors.Semantic.Nav1022DifferentViewsInConcatNotSupported;

    public override IEnumerable<Diagnostic> Analyze(ITaskDefinitionSymbol taskDefinition, AnalyzerContext context) {

        // Init Transitions betrachten
        foreach (var initNodeSymbol in taskDefinition.NodeDeclarations.OfType<IInitNodeSymbol>()) {
            foreach (var diagnostic in Analyze(initNodeSymbol.Outgoings)) {
                yield return diagnostic;
            }
        }

        // Trigger Transitions betrachten
        foreach (var triggerTransition in taskDefinition.Edges().OfType<ITriggerTransition>()) {
            foreach (var diagnostic in Analyze(triggerTransition.AsEnumerable())) {
                yield return diagnostic;
            }
        }

        // Exit Transitions betrachten
        foreach (var taskNodeSymbol in taskDefinition.NodeDeclarations.OfType<ITaskNodeSymbol>()) {
            foreach (var diagnostic in Analyze(taskNodeSymbol.Outgoings)) {
                yield return diagnostic;
            }
        }

    }

    IEnumerable<Diagnostic> Analyze(IEnumerable<IEdge> edges) {

        // In einer Continuation sind die Source Nodes immer Gui Knoten
        var guiNodeReferences = edges.SelectMany(e => e.GetReachableContinuations())
                                     .Select(c => c.SourceReference)
                                     .WhereNotNull()
                                     .DistinctBy(concatTransition => concatTransition?.Declaration)
                                     .ToImmutableArray();

        if (guiNodeReferences.Length > 1) {

            yield return new Diagnostic(
                guiNodeReferences.First().Location,
                guiNodeReferences.Skip(1).Select(nr => nr.Location),
                Descriptor
            );
        }
    }

}