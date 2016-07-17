#region Using Directives

using System;
using System.Collections.Generic;
using System.Windows.Controls;

using Microsoft.VisualStudio.Imaging.Interop;
using Pharmatechnik.Nav.Language.CodeGen;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.QuickInfo {
    sealed class SymbolQuickInfoBuilder: SymbolVisitor<IEnumerable<object>> {
        
        #region Infrastructure

        SymbolQuickInfoBuilder(SyntaxQuickinfoBuilderService syntaxQuickinfoBuilderService) {
            SyntaxQuickinfoBuilderService = syntaxQuickinfoBuilderService;
        }

        SyntaxQuickinfoBuilderService SyntaxQuickinfoBuilderService { get; }

        public static IEnumerable<object> Build(ISymbol source, SyntaxQuickinfoBuilderService syntaxQuickinfoBuilderService) {
            var builder = new SymbolQuickInfoBuilder(syntaxQuickinfoBuilderService);
            return builder.Visit(source);
        }

        protected override IEnumerable<object> DefaultVisit(ISymbol symbol) {
            yield break;
        }
        
        SymbolQuickInfoControl CreateSymbolQuickInfoControl(SyntaxNode syntax, ImageMoniker imageMoniker) {

            var control = new SymbolQuickInfoControl();
            control.CrispImage.Moniker  = imageMoniker;
            control.TextContent.Content = SyntaxQuickinfoBuilderService.ToTextBlock(syntax.SyntaxTree);

            return control;
        }

        #endregion

        #region ConnectionPoints

        public override IEnumerable<object> VisitConnectionPointReferenceSymbol(IConnectionPointReferenceSymbol connectionPointReferenceSymbol) {
            if(connectionPointReferenceSymbol.Declaration == null) {
                yield break;
            }

            foreach (var content in Visit(connectionPointReferenceSymbol.Declaration)) {
                yield return content;
            }
        }

        public override IEnumerable<object> VisitInitConnectionPointSymbol(IInitConnectionPointSymbol initConnectionPointSymbol) {

            var syntaxText = $"{initConnectionPointSymbol.Syntax.InitKeyword} {initConnectionPointSymbol.Syntax.Identifier}";
            var syntax     = Syntax.ParseInitNodeDeclaration(syntaxText);

            yield return CreateSymbolQuickInfoControl(syntax, SymbolImageMonikers.InitConnectionPoint);           
        }

        public override IEnumerable<object> VisitExitConnectionPointSymbol(IExitConnectionPointSymbol exitConnectionPointSymbol) {

            var syntaxText = $"{exitConnectionPointSymbol.Syntax.ExitKeyword} {exitConnectionPointSymbol.Syntax.Identifier}";
            var syntax     = Syntax.ParseExitNodeDeclaration(syntaxText);

            yield return CreateSymbolQuickInfoControl(syntax, SymbolImageMonikers.ExitConnectionPoint);
        }

        public override IEnumerable<object> VisitEndConnectionPointSymbol(IEndConnectionPointSymbol endConnectionPointSymbol) {

            var syntaxText = endConnectionPointSymbol.Name;
            var syntax     = Syntax.ParseEndNodeDeclaration(syntaxText);

            yield return CreateSymbolQuickInfoControl(syntax, SymbolImageMonikers.EndConnectionPoint);
        }

        #endregion

        public override IEnumerable<object> VisitTaskDeclarationSymbol(ITaskDeclarationSymbol taskDeclarationSymbol) {

            var syntaxText = $"taskref {taskDeclarationSymbol.Name}";
            var syntax     = Syntax.ParseTaskDeclaration(syntaxText);

            yield return CreateSymbolQuickInfoControl(syntax, SymbolImageMonikers.TaskDeclaration);
        }

        public override IEnumerable<object> VisitTaskDefinitionSymbol(ITaskDefinitionSymbol taskDefinitionSymbol) {

            var syntaxText = $"{taskDefinitionSymbol.Syntax.TaskKeyword} {taskDefinitionSymbol.Name}";
            var syntax     = Syntax.ParseTaskDefinition(syntaxText);

            yield return CreateSymbolQuickInfoControl(syntax, SymbolImageMonikers.TaskDefinition);
        }

        public override IEnumerable<object> VisitIncludeSymbol(IIncludeSymbol includeSymbol) {

            StackPanel panel = new StackPanel {
                Orientation=Orientation.Vertical
            };
            
            var control = new SymbolQuickInfoControl();
            control.CrispImage.Moniker  = SymbolImageMonikers.Include;
            control.TextContent.Content = SyntaxQuickinfoBuilderService.ToTextBlock(includeSymbol.FileName, SyntaxTokenClassification.Identifier);

            panel.Children.Add(control);
            
            //foreach(var taskDecl in includeSymbol.TaskDeklarations) {
            //    foreach(var elem in Visit(taskDecl).OfType<SymbolQuickInfoControl>()) {
            //        elem.Margin = new Thickness(20,0,0,0);
            //        panel.Children.Add(elem);
            //    }
            //}

            yield return panel;
        }

        #region Nodes

        public override IEnumerable<object> VisitNodeReferenceSymbol(INodeReferenceSymbol nodeReferenceSymbol) {
            if (nodeReferenceSymbol.Declaration == null) {
                yield break;
            }

            foreach(var content in Visit(nodeReferenceSymbol.Declaration)) {
                yield return content;
            }
        }

        public override IEnumerable<object> VisitInitNodeSymbol(IInitNodeSymbol initNodeSymbol) {

            var syntaxText = $"{initNodeSymbol.Syntax.InitKeyword} {initNodeSymbol.Syntax.Identifier}";
            var syntax     = Syntax.ParseInitNodeDeclaration(syntaxText);

            yield return CreateSymbolQuickInfoControl(syntax, SymbolImageMonikers.InitNode);
        }
        
        public override IEnumerable<object> VisitExitNodeSymbol(IExitNodeSymbol exitNodeSymbol) {

            var syntaxText = $"{exitNodeSymbol.Syntax.ExitKeyword} {exitNodeSymbol.Syntax.Identifier}";
            var syntax     = Syntax.ParseExitNodeDeclaration(syntaxText);

            yield return CreateSymbolQuickInfoControl(syntax, SymbolImageMonikers.ExitNode);
        }

        public override IEnumerable<object> VisitEndNodeSymbol(IEndNodeSymbol endNodeSymbol) {

            var syntaxText = endNodeSymbol.Name;
            var syntax     = Syntax.ParseEndNodeDeclaration(syntaxText);

            yield return CreateSymbolQuickInfoControl(syntax, SymbolImageMonikers.EndNode);
        }

        public override IEnumerable<object> VisitTaskNodeSymbol(ITaskNodeSymbol taskNodeSymbol) {

            var alias = taskNodeSymbol.Syntax.IdentifierAlias.IsMissing ? String.Empty: taskNodeSymbol.Syntax.IdentifierAlias.ToString();

            var syntaxText = $"{taskNodeSymbol.Syntax.TaskKeyword} {taskNodeSymbol.Syntax.Identifier} {alias}";
            var syntax     = Syntax.ParseTaskNodeDeclaration(syntaxText);

            yield return CreateSymbolQuickInfoControl(syntax, SymbolImageMonikers.TaskNode);
        }

        public override IEnumerable<object> VisitTaskNodeAliasSymbol(ITaskNodeAliasSymbol taskNodeAlias) {
            return Visit(taskNodeAlias.TaskNode);
        }

        public override IEnumerable<object> VisitChoiceNodeSymbol(IChoiceNodeSymbol choiceNodeSymbol) {

            var syntaxText = $"{choiceNodeSymbol.Syntax.ChoiceKeyword} {choiceNodeSymbol.Syntax.Identifier}";
            var syntax     = Syntax.ParseChoiceNodeDeclaration(syntaxText);

            yield return CreateSymbolQuickInfoControl(syntax, SymbolImageMonikers.ChoiceNode);
        }
        
        public override IEnumerable<object> VisitViewNodeSymbol(IViewNodeSymbol viewNodeSymbol) {

            var syntaxText = $"{viewNodeSymbol.Syntax.ViewKeyword} {viewNodeSymbol.Syntax.Identifier}";
            var syntax     = Syntax.ParseViewNodeDeclaration(syntaxText);

            yield return CreateSymbolQuickInfoControl(syntax, SymbolImageMonikers.ViewNode);
        }
        
        public override IEnumerable<object> VisitDialogNodeSymbol(IDialogNodeSymbol dialogNodeSymbol) {

            var syntaxText = $"{dialogNodeSymbol.Syntax.DialogKeyword} {dialogNodeSymbol.Syntax.Identifier}";
            var syntax = Syntax.ParseDialogNodeDeclaration(syntaxText);

            yield return CreateSymbolQuickInfoControl(syntax, SymbolImageMonikers.DialogNode);
        }

        #endregion

        public override IEnumerable<object> VisitSignalTriggerSymbol(ISignalTriggerSymbol signalTriggerSymbol) {

            var signalTriggerCodeModel = new SignalTriggerCodeModel(signalTriggerSymbol);
            
            StackPanel panel = new StackPanel {
                Orientation = Orientation.Vertical
            };

            var control = new SymbolQuickInfoControl();
            control.CrispImage.Moniker  = SymbolImageMonikers.SignalTrigger;
            control.TextContent.Content = SyntaxQuickinfoBuilderService.ToTextBlock(signalTriggerCodeModel);

            panel.Children.Add(control);

            yield return panel;
        }
    }
}