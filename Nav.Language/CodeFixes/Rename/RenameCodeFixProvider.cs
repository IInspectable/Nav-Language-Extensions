#region Using Directives

using System.Threading;
using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language.CodeFixes.Rename {

    public static class RenameCodeFixProvider {

        [CanBeNull]
        public static RenameCodeFix TryGetCodeFix(CodeFixContext context, CancellationToken cancellationToken= default(CancellationToken)) {

            var symbol = context.TryFindSymbolAtPosition();
            if (symbol == null) {
                return null;
            }

            var finder = new Visitor(symbol, context);

            var codeFix = finder.Visit(symbol);
            return codeFix;
        }

        sealed class Visitor : SymbolVisitor<RenameCodeFix> {

            public Visitor(ISymbol originatingSymbol, CodeFixContext context) {
                OriginatingSymbol  = originatingSymbol;
                Context = context;
            }

            ISymbol OriginatingSymbol { get; }
            CodeFixContext Context { get; }

            
            public override RenameCodeFix VisitInitNodeSymbol(IInitNodeSymbol initNodeSymbol) {
                // Wenn es bereits einen Alias gibt, dann funktioniert der Rename nur auf dem Alias-Symbol
                if (OriginatingSymbol == initNodeSymbol && initNodeSymbol.Alias != null) {
                    return DefaultVisit(initNodeSymbol);
                }
                return new InitNodeRenameCodeFix(initNodeSymbol, Context);
            }

            public override RenameCodeFix VisitInitNodeAliasSymbol(IInitNodeAliasSymbol initNodeAliasSymbol) {
                return new InitNodeRenameCodeFix(initNodeAliasSymbol.InitNode, Context);
            }

            public override RenameCodeFix VisitExitNodeSymbol(IExitNodeSymbol exitNodeSymbol) {
                return new ExitNodeRenameCodeFix(exitNodeSymbol, Context);
            }

            public override RenameCodeFix VisitTaskNodeSymbol(ITaskNodeSymbol taskNodeSymbol) {
                // Wenn es bereits einen Alias gibt, dann funktioniert der Rename nur auf dem Alias-Symbol
                if (OriginatingSymbol == taskNodeSymbol && taskNodeSymbol.Alias != null) {
                    return DefaultVisit(taskNodeSymbol);
                }
                return new TaskNodeRenameCodeFix(taskNodeSymbol, Context);
            }

            public override RenameCodeFix VisitTaskNodeAliasSymbol(ITaskNodeAliasSymbol taskNodeAliasSymbol) {
                return new TaskNodeRenameCodeFix(taskNodeAliasSymbol.TaskNode, Context);
            }

            public override RenameCodeFix VisitChoiceNodeSymbol(IChoiceNodeSymbol choiceNodeSymbol) {
                return new ChoiceRenameCodeFix(choiceNodeSymbol, Context);
            }

            public override RenameCodeFix VisitDialogNodeSymbol(IDialogNodeSymbol dialogNodeSymbol) {
                return new DialogNodeRenameCodeFix(dialogNodeSymbol, Context);
            }

            public override RenameCodeFix VisitViewNodeSymbol(IViewNodeSymbol viewNodeSymbol) {
                return new ViewNodeRenameCodeFix(viewNodeSymbol, Context);
            }

            public override RenameCodeFix VisitTaskDeclarationSymbol(ITaskDeclarationSymbol taskDeclarationSymbol) {
                return new TaskDeclarationRenameCodeFix(taskDeclarationSymbol, Context);
            }

            public override RenameCodeFix VisitTaskDefinitionSymbol(ITaskDefinitionSymbol taskDefinitionSymbol) {
                if (taskDefinitionSymbol.AsTaskDeclaration == null) {
                    return DefaultVisit(taskDefinitionSymbol);
                }
                return Visit(taskDefinitionSymbol.AsTaskDeclaration);
            }

            public override RenameCodeFix VisitNodeReferenceSymbol(INodeReferenceSymbol nodeReferenceSymbol) {
                if (nodeReferenceSymbol.Declaration == null) {
                    return DefaultVisit(nodeReferenceSymbol);
                }
                return Visit(nodeReferenceSymbol.Declaration);
            }
        }
    }    
}