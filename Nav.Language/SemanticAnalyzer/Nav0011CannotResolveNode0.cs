using System.Linq;
using System.Collections.Generic;

namespace Pharmatechnik.Nav.Language.SemanticAnalyzer {

     public class Nav0011CannotResolveNode0: ITaskDefinitionAnalyzer {

        public DiagnosticDescriptor Descriptor => DiagnosticDescriptors.Semantic.Nav0011CannotResolveNode0;

        public IEnumerable<Diagnostic> Analyze(ITaskDefinitionSymbol taskDefinition, AnalyzerContext context) {
            //==============================
            // Cannot resolve node '{0}'
            //==============================
            foreach (var targetReference in taskDefinition.Edges().Select(e => e.TargetReference)) {

                if (targetReference != null && targetReference.Declaration == null) {
                    yield return (new Diagnostic(
                        targetReference.Location,
                        Descriptor,
                        targetReference.Name));
                }
            }
        }

    }

}