#region Using Directives

using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language.FindReferences {

    class FindDefinitionVisitor: SymbolVisitor<ISymbol> {

        FindDefinitionVisitor(ISymbol originatingSymbol) {
            OriginatingSymbol = originatingSymbol;
        }

        public ISymbol OriginatingSymbol { get; }

        [CanBeNull]
        public static ISymbol Invoke(ISymbol symbol) {
            if (symbol == null) {
                return null;
            }

            var finder = new FindDefinitionVisitor(symbol);
            return finder.Visit(symbol);
        }

        protected override ISymbol DefaultVisit(ISymbol symbol) {
            return symbol;
        }

        public override ISymbol VisitInitNodeAliasSymbol(IInitNodeAliasSymbol initNodeAliasSymbol) {
            return Visit(initNodeAliasSymbol.InitNode);
        }

        public override ISymbol VisitNodeReferenceSymbol(INodeReferenceSymbol nodeReferenceSymbol) {

            if (nodeReferenceSymbol.Declaration != null) {
                return Visit(nodeReferenceSymbol.Declaration);
            }

            return DefaultVisit(nodeReferenceSymbol);
        }

        public override ISymbol VisitTaskNodeAliasSymbol(ITaskNodeAliasSymbol taskNodeAliasSymbol) {
            return Visit(taskNodeAliasSymbol.TaskNode);
        }

    }

}