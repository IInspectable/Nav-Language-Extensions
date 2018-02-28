﻿using System.Linq;
using System.Collections.Generic;

namespace Pharmatechnik.Nav.Language.SemanticAnalyzer {

    public class Nav0115DialogNode0HasNoOutgoingEdges: ITaskDefinitionAnalyzer {

        public DiagnosticDescriptor Descriptor => DiagnosticDescriptors.Semantic.Nav0115DialogNode0HasNoOutgoingEdges;

        public IEnumerable<Diagnostic> Analyze(ITaskDefinitionSymbol taskDefinition, AnalyzerContext context) {
            //==============================
            // The dialog node '{0}' has no outgoing edges
            //==============================
            foreach (var dialogNode in taskDefinition.NodeDeclarations.OfType<IDialogNodeSymbol>()) {

                if (dialogNode.Incomings.Any() && !dialogNode.Outgoings.Any()) {

                    yield return new Diagnostic(
                        dialogNode.Location,
                        Descriptor,
                        dialogNode.Name);
                }
            }
        }

    }

}