#region Using Directives

using JetBrains.Annotations;

using Pharmatechnik.Nav.Language.Text;

#endregion

namespace Pharmatechnik.Nav.Language.FindReferences {

    class FindRootDefinitionVisitor: SymbolVisitor<DefinitionEntry> {

        FindRootDefinitionVisitor(ISymbol originatingSymbol) {
            OriginatingSymbol = originatingSymbol;
        }

        public ISymbol OriginatingSymbol { get; }

        [CanBeNull]
        public static DefinitionEntry Invoke(ISymbol symbol) {
            if (symbol == null) {
                return null;
            }

            var finder = new FindRootDefinitionVisitor(symbol);
            return finder.Visit(symbol);
        }

        protected override DefinitionEntry DefaultVisit(ISymbol symbol) {
            return new DefinitionEntry(symbol, symbol.ToDisplayParts());
        }

        public override DefinitionEntry VisitInitNodeAliasSymbol(IInitNodeAliasSymbol initNodeAliasSymbol) {
            return Visit(initNodeAliasSymbol.InitNode);
        }

        public override DefinitionEntry VisitNodeReferenceSymbol(INodeReferenceSymbol nodeReferenceSymbol) {

            if (nodeReferenceSymbol.Declaration != null) {
                return Visit(nodeReferenceSymbol.Declaration);
            }

            return DefaultVisit(nodeReferenceSymbol);
        }

        public override DefinitionEntry VisitTaskNodeAliasSymbol(ITaskNodeAliasSymbol taskNodeAliasSymbol) {
            return Visit(taskNodeAliasSymbol.TaskNode);
        }

    }

}