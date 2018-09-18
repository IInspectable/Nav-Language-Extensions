#region Using Directives

using System.Collections.Generic;
using System.Linq;

#endregion

namespace Pharmatechnik.Nav.Language.FindReferences {

    sealed class FindReferencesVisitor: SymbolVisitor<IEnumerable<ISymbol>> {

        public static IEnumerable<ISymbol> Invoke(DefinitionItem definition) {

            if (definition?.Symbol == null) {
                return Enumerable.Empty<ISymbol>();
            }

            var finder = new FindReferencesVisitor();
            return finder.Visit(definition.Symbol);
        }

        protected override IEnumerable<ISymbol> DefaultVisit(ISymbol symbol) {
            yield break;
        }

        // TODO der eigentlich wichtige Part...
        //public override IEnumerable<ISymbol> VisitTaskDefinitionSymbol(ITaskDefinitionSymbol taskDefinitionSymbol) {

        //    return base.VisitTaskDefinitionSymbol(taskDefinitionSymbol);
        //}

        public override IEnumerable<ISymbol> VisitInitNodeSymbol(IInitNodeSymbol initNodeSymbol) {

            foreach (var transition in initNodeSymbol.Outgoings) {
                yield return transition.SourceReference;
            }
        }

        public override IEnumerable<ISymbol> VisitInitNodeAliasSymbol(IInitNodeAliasSymbol initNodeAliasSymbol) {
            return Visit(initNodeAliasSymbol.InitNode);
        }

        public override IEnumerable<ISymbol> VisitTaskNodeSymbol(ITaskNodeSymbol taskNodeSymbol) {

            foreach (var exitTransition in taskNodeSymbol.Outgoings) {
                yield return exitTransition.SourceReference;
            }

            foreach (var edge in taskNodeSymbol.Incomings) {
                yield return edge.TargetReference;
            }
        }

        public override IEnumerable<ISymbol> VisitExitNodeSymbol(IExitNodeSymbol exitNodeSymbol) {

            foreach (var edge in exitNodeSymbol.Incomings) {
                yield return edge.TargetReference;
            }
        }

        public override IEnumerable<ISymbol> VisitEndNodeSymbol(IEndNodeSymbol endNodeSymbol) {

            foreach (var edge in endNodeSymbol.Incomings) {
                yield return edge.TargetReference;
            }
        }

        public override IEnumerable<ISymbol> VisitDialogNodeSymbol(IDialogNodeSymbol dialogNodeSymbol) {

            foreach (var transition in dialogNodeSymbol.Outgoings) {
                yield return transition.SourceReference;
            }

            foreach (var edge in dialogNodeSymbol.Incomings) {
                yield return edge.TargetReference;
            }
        }

        public override IEnumerable<ISymbol> VisitViewNodeSymbol(IViewNodeSymbol viewNodeSymbol) {

            foreach (var transition in viewNodeSymbol.Outgoings) {
                yield return transition.SourceReference;
            }

            foreach (var edge in viewNodeSymbol.Incomings) {
                yield return edge.TargetReference;
            }
        }

        public override IEnumerable<ISymbol> VisitChoiceNodeSymbol(IChoiceNodeSymbol choiceNodeSymbol) {

            foreach (var transition in choiceNodeSymbol.Outgoings) {
                yield return transition.SourceReference;
            }

            foreach (var edge in choiceNodeSymbol.Incomings) {
                yield return edge.TargetReference;
            }
        }

        public override IEnumerable<ISymbol> VisitTaskDeclarationSymbol(ITaskDeclarationSymbol taskDeclarationSymbol) {

            foreach (var taskNode in taskDeclarationSymbol.References) {
                yield return taskNode;
            }
        }

    }

}