#region Using Directives

using System.Collections.Immutable;

using Pharmatechnik.Nav.Language.CodeGen;

#endregion

namespace Pharmatechnik.Nav.Language.Text {

    class DisplayPartsVisitor: SymbolVisitor<ImmutableArray<ClassifiedText>> {

        public static ImmutableArray<ClassifiedText> Invoke(ISymbol source) {
            var builder = new DisplayPartsVisitor();
            return builder.Visit(source);
        }

        protected override ImmutableArray<ClassifiedText> DefaultVisit(ISymbol symbol) {
            return ImmutableArray<ClassifiedText>.Empty;
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
                ClassifiedTexts.Keyword(SyntaxFacts.InitKeyword),
                ClassifiedTexts.Space,
                ClassifiedTexts.Identifier(initConnectionPointSymbol.Syntax.Identifier.ToString())
            );

        }

        public override ImmutableArray<ClassifiedText> VisitExitConnectionPointSymbol(IExitConnectionPointSymbol exitConnectionPointSymbol) {

            return CreateClassifiedText(
                ClassifiedTexts.Keyword(SyntaxFacts.ExitKeyword),
                ClassifiedTexts.Space,
                ClassifiedTexts.Identifier(exitConnectionPointSymbol.Syntax.Identifier.ToString())
            );
        }

        public override ImmutableArray<ClassifiedText> VisitEndConnectionPointSymbol(IEndConnectionPointSymbol endConnectionPointSymbol) {

            return CreateClassifiedText(
                ClassifiedTexts.Keyword(SyntaxFacts.EndKeyword)
            );
        }

        #endregion

        public override ImmutableArray<ClassifiedText> VisitTaskDeclarationSymbol(ITaskDeclarationSymbol taskDeclarationSymbol) {

            return CreateClassifiedText(
                ClassifiedTexts.Keyword(SyntaxFacts.TaskrefKeyword),
                ClassifiedTexts.Space,
                ClassifiedTexts.TaskName(taskDeclarationSymbol.Name)
            );

        }

        public override ImmutableArray<ClassifiedText> VisitTaskDefinitionSymbol(ITaskDefinitionSymbol taskDefinitionSymbol) {

            return CreateClassifiedText(
                ClassifiedTexts.Keyword(SyntaxFacts.TaskKeyword),
                ClassifiedTexts.Space,
                ClassifiedTexts.TaskName(taskDefinitionSymbol.Name)
            );
        }

        public override ImmutableArray<ClassifiedText> VisitIncludeSymbol(IIncludeSymbol includeSymbol) {
            return CreateClassifiedText(
                ClassifiedTexts.Keyword(SyntaxFacts.TaskrefKeyword),
                ClassifiedTexts.Space,
                ClassifiedTexts.Identifier(includeSymbol.FileName) // Sieht als Identifier besser aus...
            );
        }

        #region Nodes

        public override ImmutableArray<ClassifiedText> VisitNodeReferenceSymbol(INodeReferenceSymbol nodeReferenceSymbol) {
            if (nodeReferenceSymbol.Declaration != null) {
                return Visit(nodeReferenceSymbol.Declaration);
            }

            return DefaultVisit(nodeReferenceSymbol);
        }

        public override ImmutableArray<ClassifiedText> VisitInitNodeAliasSymbol(IInitNodeAliasSymbol initNodeAliasSymbol) {
            return Visit(initNodeAliasSymbol.InitNode);
        }

        public override ImmutableArray<ClassifiedText> VisitInitNodeSymbol(IInitNodeSymbol initNodeSymbol) {

            if (initNodeSymbol.Alias?.Name == null) {
                return CreateClassifiedText(
                    ClassifiedTexts.Keyword(SyntaxFacts.InitKeyword)
                );
            }

            return CreateClassifiedText(
                ClassifiedTexts.Keyword(SyntaxFacts.InitKeyword),
                ClassifiedTexts.Space,
                ClassifiedTexts.Identifier(initNodeSymbol.Alias.Name)
            );
        }

        public override ImmutableArray<ClassifiedText> VisitTaskNodeAliasSymbol(ITaskNodeAliasSymbol taskNodeAlias) {
            return Visit(taskNodeAlias.TaskNode);
        }

        public override ImmutableArray<ClassifiedText> VisitTaskNodeSymbol(ITaskNodeSymbol taskNodeSymbol) {

            if (taskNodeSymbol.Declaration?.Name == null) {
                return DefaultVisit(taskNodeSymbol);
            }

            if (taskNodeSymbol.Alias != null) {
                return CreateClassifiedText(
                    ClassifiedTexts.Keyword(SyntaxFacts.TaskKeyword),
                    ClassifiedTexts.Space,
                    ClassifiedTexts.TaskName(taskNodeSymbol.Declaration.Name),
                    ClassifiedTexts.Space,
                    ClassifiedTexts.Identifier(taskNodeSymbol.Name)
                );
            }

            return CreateClassifiedText(
                ClassifiedTexts.Keyword(SyntaxFacts.TaskKeyword),
                ClassifiedTexts.Space,
                ClassifiedTexts.TaskName(taskNodeSymbol.Declaration.Name)
            );

        }

        public override ImmutableArray<ClassifiedText> VisitChoiceNodeSymbol(IChoiceNodeSymbol choiceNodeSymbol) {
            return CreateClassifiedText(
                ClassifiedTexts.Keyword(SyntaxFacts.ChoiceKeyword),
                ClassifiedTexts.Space,
                ClassifiedTexts.Identifier(choiceNodeSymbol.Name)
            );
        }

        public override ImmutableArray<ClassifiedText> VisitDialogNodeSymbol(IDialogNodeSymbol dialogNodeSymbol) {
            return CreateClassifiedText(
                ClassifiedTexts.Keyword(SyntaxFacts.DialogKeyword),
                ClassifiedTexts.Space,
                ClassifiedTexts.FormName(dialogNodeSymbol.Name)
            );
        }

        public override ImmutableArray<ClassifiedText> VisitViewNodeSymbol(IViewNodeSymbol viewNodeSymbol) {
            return CreateClassifiedText(
                ClassifiedTexts.Keyword(SyntaxFacts.ViewKeyword),
                ClassifiedTexts.Space,
                ClassifiedTexts.FormName(viewNodeSymbol.Name)
            );
        }

        public override ImmutableArray<ClassifiedText> VisitExitNodeSymbol(IExitNodeSymbol exitNodeSymbol) {
            return CreateClassifiedText(
                ClassifiedTexts.Keyword(SyntaxFacts.ExitKeyword),
                ClassifiedTexts.Space,
                ClassifiedTexts.Identifier(exitNodeSymbol.Name)
            );
        }

        public override ImmutableArray<ClassifiedText> VisitEndNodeSymbol(IEndNodeSymbol endNodeSymbol) {
            return CreateClassifiedText(
                ClassifiedTexts.Keyword(SyntaxFacts.EndKeyword)
            );
        }

        #endregion

        public override ImmutableArray<ClassifiedText> VisitEdgeModeSymbol(IEdgeModeSymbol edgeModeSymbol) {
            return CreateClassifiedText(
                ClassifiedTexts.Keyword(edgeModeSymbol.Verb)
            );
        }

        public override ImmutableArray<ClassifiedText> VisitSpontaneousTriggerSymbol(ISpontaneousTriggerSymbol spontaneousTriggerSymbol) {
            return CreateClassifiedText(
                ClassifiedTexts.Keyword(SyntaxFacts.SpontaneousKeyword)
            );
        }

        public override ImmutableArray<ClassifiedText> VisitSignalTriggerSymbol(ISignalTriggerSymbol signalTriggerSymbol) {

            var codeInfo = SignalTriggerCodeInfo.FromSignalTrigger(signalTriggerSymbol);

            return CreateClassifiedText(
                ClassifiedTexts.TaskName(codeInfo.Task.WfsTypeName),
                ClassifiedTexts.Punctuation("."),
                ClassifiedTexts.Identifier(codeInfo.TriggerLogicMethodName),
                ClassifiedTexts.Punctuation("()")
            );
        }

        ImmutableArray<ClassifiedText> CreateClassifiedText(params ClassifiedText[] parts) {
            return parts.ToImmutableArray();
        }

    }

}