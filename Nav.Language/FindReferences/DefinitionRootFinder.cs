#region Using Directives

using System.Collections.Immutable;

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
            return null;
        }

        public override DefinitionEntry VisitTaskDefinitionSymbol(ITaskDefinitionSymbol taskDefinitionSymbol) {

            return CreateDefinitionEntry(taskDefinitionSymbol,
                                         ClassifiedText.Keyword(SyntaxFacts.TaskKeyword),
                                         ClassifiedText.Space,
                                         ClassifiedText.TaskName(taskDefinitionSymbol.Name)
            );

        }

        public override DefinitionEntry VisitTaskDeclarationSymbol(ITaskDeclarationSymbol taskDeclarationSymbol) {

            return CreateDefinitionEntry(taskDeclarationSymbol,
                                         ClassifiedText.Keyword(SyntaxFacts.TaskrefKeyword),
                                         ClassifiedText.Space,
                                         ClassifiedText.TaskName(taskDeclarationSymbol.Name)
            );

        }

        public override DefinitionEntry VisitInitNodeAliasSymbol(IInitNodeAliasSymbol initNodeAliasSymbol) {
            return Visit(initNodeAliasSymbol.InitNode);
        }

        public override DefinitionEntry VisitInitNodeSymbol(IInitNodeSymbol initNodeSymbol) {

            if (initNodeSymbol.Alias?.Name == null) {
                return CreateDefinitionEntry(initNodeSymbol,
                                             ClassifiedText.Keyword(SyntaxFacts.InitKeyword));
            }

            return CreateDefinitionEntry(initNodeSymbol,
                                         ClassifiedText.Keyword(SyntaxFacts.InitKeyword),
                                         ClassifiedText.Space,
                                         ClassifiedText.Identifier(initNodeSymbol.Alias.Name)
            );
        }

        public override DefinitionEntry VisitTaskNodeSymbol(ITaskNodeSymbol taskNodeSymbol) {

            if (taskNodeSymbol.Declaration?.Name == null) {
                return DefaultVisit(taskNodeSymbol);
            }

            if (taskNodeSymbol.Alias != null) {
                return CreateDefinitionEntry(taskNodeSymbol,
                                             ClassifiedText.Keyword(SyntaxFacts.TaskKeyword),
                                             ClassifiedText.Space,
                                             ClassifiedText.TaskName(taskNodeSymbol.Declaration.Name),
                                             ClassifiedText.Space,
                                             ClassifiedText.Identifier(taskNodeSymbol.Name)
                );
            }

            return CreateDefinitionEntry(taskNodeSymbol,
                                         ClassifiedText.Keyword(SyntaxFacts.TaskKeyword),
                                         ClassifiedText.Space,
                                         ClassifiedText.TaskName(taskNodeSymbol.Declaration.Name)
            );

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

        static DefinitionEntry CreateDefinitionEntry(ISymbol symbol, params ClassifiedText[] parts) {
            return new DefinitionEntry(symbol, parts.ToImmutableArray());
        }

    }

}