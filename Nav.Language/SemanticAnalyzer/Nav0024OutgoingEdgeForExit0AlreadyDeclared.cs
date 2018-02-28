using System.Linq;
using System.Collections.Generic;

namespace Pharmatechnik.Nav.Language.SemanticAnalyzer {

    public class Nav0024OutgoingEdgeForExit0AlreadyDeclared: ITaskDefinitionAnalyzer {

        public DiagnosticDescriptor Descriptor => DiagnosticDescriptors.Semantic.Nav0024OutgoingEdgeForExit0AlreadyDeclared;

        public IEnumerable<Diagnostic> Analyze(ITaskDefinitionSymbol taskDefinition, AnalyzerContext context) {
            //==============================
            //  An outgoing edge for exit '{0}' is already declared
            //==============================
            foreach (var taskNode in taskDefinition.NodeDeclarations.OfType<ITaskNodeSymbol>()) {

                if (taskNode.References.Any() && taskNode.Declaration != null) {

                    var actualExits = taskNode.Outgoings
                                              .Select(et => et.ConnectionPointReference)
                                              .Where(cp => cp != null)
                                              .ToList();

                    foreach (var duplicates in actualExits.GroupBy(e => e.Name).Where(g => g.Count() > 1)) {
                        yield return new Diagnostic(
                            duplicates.First().Location,
                            duplicates.Skip(1).Select(d => d.Location),
                            Descriptor,
                            duplicates.First().Name);
                    }

                }
            }
        }

    }

}