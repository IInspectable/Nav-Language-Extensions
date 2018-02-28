using System.Linq;
using System.Collections.Generic;

namespace Pharmatechnik.Nav.Language.Analyzer {

    public class Nav1012TaskNode0NotRequired: ITaskDefinitionAnalyzer {

        public DiagnosticDescriptor Descriptor => DiagnosticDescriptors.DeadCode.Nav1012TaskNode0NotRequired;

        public IEnumerable<Diagnostic> Analyze(ITaskDefinitionSymbol taskDefinition, AnalyzerContext context) {
            foreach (var taskNode in taskDefinition.NodeDeclarations.OfType<ITaskNodeSymbol>()) {

                if (!taskNode.References.Any()) {

                    yield return new Diagnostic(
                        taskNode.Syntax.GetLocation(),
                        Descriptor,
                        taskNode.Name);
                }
            }

        }

    }

}