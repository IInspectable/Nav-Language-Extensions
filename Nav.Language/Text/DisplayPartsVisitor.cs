#region Using Directives

using Pharmatechnik.Nav.Language.Text;

using System.Collections.Immutable;

#endregion

namespace Pharmatechnik.Nav.Language {

    class DisplayPartsVisitor: SymbolVisitor<ImmutableArray<ClassifiedText>> {

        DisplayPartsVisitor(ISymbol originatingSymbol) {
            OriginatingSymbol = originatingSymbol;
        }

        ISymbol OriginatingSymbol { get; }

        public static ImmutableArray<ClassifiedText> Invoke(ISymbol source) {
            var builder = new DisplayPartsVisitor(source);
            return builder.Visit(source);
        }

        #region ConnectionPoints

        public override ImmutableArray<ClassifiedText> VisitConnectionPointReferenceSymbol(IConnectionPointReferenceSymbol connectionPointReferenceSymbol) {
            if (connectionPointReferenceSymbol.Declaration == null) {
                return default;
            }

            return Visit(connectionPointReferenceSymbol.Declaration);
        }

        public override ImmutableArray<ClassifiedText> VisitInitConnectionPointSymbol(IInitConnectionPointSymbol initConnectionPointSymbol) {

            return CreateClassifiedText(
                ClassifiedText.Keyword(SyntaxFacts.InitKeyword),
                ClassifiedText.Space,
                ClassifiedText.Identifier(initConnectionPointSymbol.Syntax.Identifier.ToString())
            );

        }

        public override ImmutableArray<ClassifiedText> VisitExitConnectionPointSymbol(IExitConnectionPointSymbol exitConnectionPointSymbol) {

            return CreateClassifiedText(
                ClassifiedText.Keyword(SyntaxFacts.ExitKeyword),
                ClassifiedText.Space,
                ClassifiedText.Identifier(exitConnectionPointSymbol.Syntax.Identifier.ToString())
            );
        }

        public override ImmutableArray<ClassifiedText> VisitEndConnectionPointSymbol(IEndConnectionPointSymbol endConnectionPointSymbol) {

            return CreateClassifiedText(
                ClassifiedText.Keyword(SyntaxFacts.EndKeyword)
            );
        }

        #endregion

        public override ImmutableArray<ClassifiedText> VisitTaskDeclarationSymbol(ITaskDeclarationSymbol taskDeclarationSymbol) {

            return CreateClassifiedText(
                ClassifiedText.Keyword(SyntaxFacts.TaskrefKeyword),
                ClassifiedText.Space,
                ClassifiedText.TaskName(taskDeclarationSymbol.Name)
            );

        }

        public override ImmutableArray<ClassifiedText> VisitTaskDefinitionSymbol(ITaskDefinitionSymbol taskDefinitionSymbol) {

            return CreateClassifiedText(
                ClassifiedText.Keyword(SyntaxFacts.TaskKeyword),
                ClassifiedText.Space,
                ClassifiedText.TaskName(taskDefinitionSymbol.Name)
            );
        }

        public override ImmutableArray<ClassifiedText> VisitIncludeSymbol(IIncludeSymbol includeSymbol) {
            return CreateClassifiedText(
                ClassifiedText.Keyword(SyntaxFacts.TaskrefKeyword),
                ClassifiedText.Space,
                ClassifiedText.StringLiteral(includeSymbol.FileName)
            );
        }

        #region Nodes

        public override ImmutableArray<ClassifiedText> VisitInitNodeAliasSymbol(IInitNodeAliasSymbol initNodeAliasSymbol) {
            return Visit(initNodeAliasSymbol.InitNode);
        }

        public override ImmutableArray<ClassifiedText> VisitInitNodeSymbol(IInitNodeSymbol initNodeSymbol) {

            if (initNodeSymbol.Alias?.Name == null) {
                return CreateClassifiedText(
                    ClassifiedText.Keyword(SyntaxFacts.InitKeyword)
                );
            }

            return CreateClassifiedText(
                ClassifiedText.Keyword(SyntaxFacts.InitKeyword),
                ClassifiedText.Space,
                ClassifiedText.Identifier(initNodeSymbol.Alias.Name)
            );
        }

        public override ImmutableArray<ClassifiedText> VisitTaskNodeSymbol(ITaskNodeSymbol taskNodeSymbol) {

            if (taskNodeSymbol.Declaration?.Name == null) {
                return DefaultVisit(taskNodeSymbol);
            }

            if (taskNodeSymbol.Alias != null) {
                return CreateClassifiedText(
                    ClassifiedText.Keyword(SyntaxFacts.TaskKeyword),
                    ClassifiedText.Space,
                    ClassifiedText.TaskName(taskNodeSymbol.Declaration.Name),
                    ClassifiedText.Space,
                    ClassifiedText.Identifier(taskNodeSymbol.Name)
                );
            }

            return CreateClassifiedText(
                ClassifiedText.Keyword(SyntaxFacts.TaskKeyword),
                ClassifiedText.Space,
                ClassifiedText.TaskName(taskNodeSymbol.Declaration.Name)
            );

        }

        public override ImmutableArray<ClassifiedText> VisitChoiceNodeSymbol(IChoiceNodeSymbol choiceNodeSymbol) {
            return CreateClassifiedText(
                ClassifiedText.Keyword(SyntaxFacts.ChoiceKeyword),
                ClassifiedText.Space,
                ClassifiedText.Identifier(choiceNodeSymbol.Name)
            );
        }

        public override ImmutableArray<ClassifiedText> VisitDialogNodeSymbol(IDialogNodeSymbol dialogNodeSymbol) {
            return CreateClassifiedText(
                ClassifiedText.Keyword(SyntaxFacts.DialogKeyword),
                ClassifiedText.Space,
                ClassifiedText.FormName(dialogNodeSymbol.Name)
            );
        }

        public override ImmutableArray<ClassifiedText> VisitViewNodeSymbol(IViewNodeSymbol viewNodeSymbol) {
            return CreateClassifiedText(
                ClassifiedText.Keyword(SyntaxFacts.ViewKeyword),
                ClassifiedText.Space,
                ClassifiedText.FormName(viewNodeSymbol.Name)
            );
        }

        public override ImmutableArray<ClassifiedText> VisitExitNodeSymbol(IExitNodeSymbol exitNodeSymbol) {
            return CreateClassifiedText(
                ClassifiedText.Keyword(SyntaxFacts.ExitKeyword),
                ClassifiedText.Space,
                ClassifiedText.Identifier(exitNodeSymbol.Name)
            );
        }

        public override ImmutableArray<ClassifiedText> VisitEndNodeSymbol(IEndNodeSymbol endNodeSymbol) {
            return CreateClassifiedText(
                ClassifiedText.Keyword(SyntaxFacts.EndKeyword)
            );
        }

        #endregion

        ImmutableArray<ClassifiedText> CreateClassifiedText(params ClassifiedText[] parts) {
            return parts.ToImmutableArray();
        }

    }

}