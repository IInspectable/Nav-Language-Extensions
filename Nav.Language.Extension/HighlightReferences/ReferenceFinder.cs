#region Using Directives

using System.Collections.Generic;
using System.Linq;
using Pharmatechnik.Nav.Language.Extension.Options;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.HighlightReferences {

    // Das erste zurückgeliferte Symbol hat immer den Charakter der "Definition", alle weiteren
    // stellen die Referenzen auf diese Definition dar
    sealed class ReferenceFinder : SymbolVisitor<IEnumerable<ISymbol>> {

        readonly IAdvancedOptions _advancedOptions;

        ReferenceFinder(IAdvancedOptions advancedOptions) {
            _advancedOptions = advancedOptions;
        }

        public static IEnumerable<ISymbol> FindReferences(ISymbol symbol, IAdvancedOptions advancedOptions) {

            if( !advancedOptions.HighlightReferencesUnderCursor) {
                return Enumerable.Empty<ISymbol>();
            }

            var referenceRoot = ReferenceRootFinder.FindRoot(symbol);
            var finder = new ReferenceFinder(advancedOptions);
            return finder.Visit(referenceRoot);
        }

        protected override IEnumerable<ISymbol> DefaultVisit(ISymbol symbol) {
            yield break;
        }

        public override IEnumerable<ISymbol> VisitTaskNodeSymbol(ITaskNodeSymbol taskNodeSymbol) {

            yield return taskNodeSymbol;

            if(taskNodeSymbol.Alias != null) {
                yield return taskNodeSymbol.Alias;
            }

            foreach(var exitTransition in taskNodeSymbol.Outgoings) {
                yield return exitTransition.Source;
            }

            foreach (var edge in taskNodeSymbol.Incomings) {
                yield return edge.Target;
            }
        }

        public override IEnumerable<ISymbol> VisitInitNodeSymbol(IInitNodeSymbol initNodeSymbol) {

            yield return initNodeSymbol;

            foreach (var transition in initNodeSymbol.Outgoings) {
                yield return transition.Source;
            }           
        }

        public override IEnumerable<ISymbol> VisitExitNodeSymbol(IExitNodeSymbol exitNodeSymbol) {

            yield return exitNodeSymbol;

            foreach (var edge in exitNodeSymbol.Incomings) {
                yield return edge.Target;
            }
        }

        public override IEnumerable<ISymbol> VisitEndNodeSymbol(IEndNodeSymbol endNodeSymbol) {

            yield return endNodeSymbol;

            foreach (var edge in endNodeSymbol.Incomings) {
                yield return edge.Target;
            }
        }

        public override IEnumerable<ISymbol> VisitDialogNodeSymbol(IDialogNodeSymbol dialogNodeSymbol) {

            yield return dialogNodeSymbol;

            foreach (var transition in dialogNodeSymbol.Outgoings) {
                yield return transition.Source;
            }

            foreach (var edge in dialogNodeSymbol.Incomings) {
                yield return edge.Target;
            }
        }

        public override IEnumerable<ISymbol> VisitViewNodeSymbol(IViewNodeSymbol viewNodeSymbol) {

            yield return viewNodeSymbol;

            foreach (var transition in viewNodeSymbol.Outgoings) {
                yield return transition.Source;
            }

            foreach (var edge in viewNodeSymbol.Incomings) {
                yield return edge.Target;
            }
        }

        public override IEnumerable<ISymbol> VisitChoiceNodeSymbol(IChoiceNodeSymbol choiceNodeSymbol) {

            yield return choiceNodeSymbol;

            foreach (var transition in choiceNodeSymbol.Outgoings) {
                yield return transition.Source;
            }

            foreach (var edge in choiceNodeSymbol.Incomings) {
                yield return edge.Target;
            }
        }

        public override IEnumerable<ISymbol> VisitTaskDeclarationSymbol(ITaskDeclarationSymbol taskDeclarationSymbol) {

            yield return taskDeclarationSymbol;
            
            foreach (var reference in taskDeclarationSymbol.References.SelectMany(VisitTaskNodeSymbol)) {
                yield return reference;
            }
        }

        public override IEnumerable<ISymbol> VisitIncludeSymbol(IIncludeSymbol includeSymbol) {

            if (!_advancedOptions.HighlightReferencesUnderInclude) {
                yield break;
            }

            yield return includeSymbol;

            foreach(var taskNode in includeSymbol.TaskDeklarations.SelectMany(td => td.References)) {
                yield return taskNode;
            }
        }
    }
}