using System.Collections.Generic;

using JetBrains.Annotations;

namespace Pharmatechnik.Nav.Language.FindReferences {

    class FindRootDefinitionVisitor: SymbolVisitor<ISymbol> {

        FindRootDefinitionVisitor(ISymbol originatingSymbol) {
            OriginatingSymbol = originatingSymbol;
        }

        public ISymbol OriginatingSymbol { get; }

        [CanBeNull]
        public static ISymbol Invoke(ISymbol symbol) {
            if (symbol == null) {
                return null;
            }

            var finder = new FindRootDefinitionVisitor(symbol);
            return finder.Visit(symbol);
        }

        protected override ISymbol DefaultVisit(ISymbol symbol) {
            return symbol;
        }

        public override ISymbol VisitTaskDefinitionSymbol(ITaskDefinitionSymbol taskDefinitionSymbol) {

            var taskDeclaration = taskDefinitionSymbol.AsTaskDeclaration;
            if (taskDeclaration?.IsIncluded == false) {
                return Visit(taskDeclaration);
            }

            return DefaultVisit(taskDefinitionSymbol);
        }

        public override ISymbol VisitInitNodeAliasSymbol(IInitNodeAliasSymbol initNodeAliasSymbol) {
            return Visit(initNodeAliasSymbol.InitNode);
        }

        public override ISymbol VisitTaskNodeSymbol(ITaskNodeSymbol taskNodeSymbol) {
            // Wenn die Tasknode selbst der Ursprung ist, oder es keinen Alias gibt, dann laufen wir hoch zur Deklaration - sofern sie in unserem File liegt
            if ((OriginatingSymbol == taskNodeSymbol || taskNodeSymbol.Alias == null) && taskNodeSymbol.Declaration?.IsIncluded == false) {
                return Visit(taskNodeSymbol.Declaration);
            }

            return DefaultVisit(taskNodeSymbol);
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

    //class ReferenceFinder: SymbolVisitor<IEnumerable<ReferenceEntry> {

    //}

}