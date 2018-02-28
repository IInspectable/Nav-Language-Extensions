using System.Linq;
using System.Collections.Generic;

namespace Pharmatechnik.Nav.Language.SemanticAnalyzer {

    public class Nav1009ChoiceNode0NotRequired: ITaskDefinitionAnalyzer {

        public DiagnosticDescriptor Descriptor => DiagnosticDescriptors.DeadCode.Nav1009ChoiceNode0NotRequired;

        public IEnumerable<Diagnostic> Analyze(ITaskDefinitionSymbol taskDefinition, AnalyzerContext context) {
            foreach (var choiceNode in taskDefinition.NodeDeclarations.OfType<IChoiceNodeSymbol>()) {

                if (!choiceNode.References.Any()) {

                    yield return new Diagnostic(
                        choiceNode.Syntax.GetLocation(),
                        Descriptor,
                        choiceNode.Name);
                }

            }

        }

    }

}