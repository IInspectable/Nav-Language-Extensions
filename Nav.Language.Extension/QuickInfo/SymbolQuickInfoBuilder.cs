#region Using Directives

using System;
using System.Collections.Generic;
using System.Windows.Controls;

using Microsoft.VisualStudio.Imaging.Interop;
using Pharmatechnik.Nav.Language.CodeGen;
using Pharmatechnik.Nav.Language.Extension.Images;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.QuickInfo {
    sealed class SymbolQuickInfoBuilder: SymbolVisitor<IEnumerable<object>> {
        
        #region Infrastructure

        SymbolQuickInfoBuilder(ISymbol originatingSymbol, SyntaxQuickinfoBuilderService syntaxQuickinfoBuilderService) {
            OriginatingSymbol = originatingSymbol;
            SyntaxQuickinfoBuilderService = syntaxQuickinfoBuilderService;
        }

        ISymbol OriginatingSymbol { get; }
        SyntaxQuickinfoBuilderService SyntaxQuickinfoBuilderService { get; }

        public static IEnumerable<object> Build(ISymbol source, SyntaxQuickinfoBuilderService syntaxQuickinfoBuilderService) {
            var builder = new SymbolQuickInfoBuilder(source, syntaxQuickinfoBuilderService);
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

            yield return CreateSymbolQuickInfoControl(syntax, ImageMonikers.FromSymbol(initConnectionPointSymbol));           
        }

        public override IEnumerable<object> VisitExitConnectionPointSymbol(IExitConnectionPointSymbol exitConnectionPointSymbol) {

            var syntaxText = $"{exitConnectionPointSymbol.Syntax.ExitKeyword} {exitConnectionPointSymbol.Syntax.Identifier}";
            var syntax     = Syntax.ParseExitNodeDeclaration(syntaxText);

            yield return CreateSymbolQuickInfoControl(syntax, ImageMonikers.FromSymbol(exitConnectionPointSymbol));
        }

        public override IEnumerable<object> VisitEndConnectionPointSymbol(IEndConnectionPointSymbol endConnectionPointSymbol) {

            var syntaxText = endConnectionPointSymbol.Name;
            var syntax     = Syntax.ParseEndNodeDeclaration(syntaxText);

            yield return CreateSymbolQuickInfoControl(syntax, ImageMonikers.FromSymbol(endConnectionPointSymbol));
        }

        #endregion

        public override IEnumerable<object> VisitTaskDeclarationSymbol(ITaskDeclarationSymbol taskDeclarationSymbol) {

            var syntaxText = $"taskref {taskDeclarationSymbol.Name}";
            var syntax     = Syntax.ParseTaskDeclaration(syntaxText);

            yield return CreateSymbolQuickInfoControl(syntax, ImageMonikers.FromSymbol(taskDeclarationSymbol));
        }

        public override IEnumerable<object> VisitTaskDefinitionSymbol(ITaskDefinitionSymbol taskDefinitionSymbol) {

            var syntaxText = $"{taskDefinitionSymbol.Syntax.TaskKeyword} {taskDefinitionSymbol.Name}";
            var syntax     = Syntax.ParseTaskDefinition(syntaxText);

            yield return CreateSymbolQuickInfoControl(syntax, ImageMonikers.FromSymbol(taskDefinitionSymbol));
        }

        public override IEnumerable<object> VisitIncludeSymbol(IIncludeSymbol includeSymbol) {

            StackPanel panel = new StackPanel {
                Orientation=Orientation.Vertical
            };
            
            var control = new SymbolQuickInfoControl();
            control.CrispImage.Moniker  = ImageMonikers.FromSymbol(includeSymbol);
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
            // Wir zeigen keinen Tooltip für das init Keyword an, wenn es einen Alias gibt
            if(OriginatingSymbol == initNodeSymbol && initNodeSymbol.Alias != null) {
                yield break;
            }
            var syntaxText = $"{initNodeSymbol.Syntax.InitKeyword} {initNodeSymbol.Syntax.Identifier}";
            var syntax     = Syntax.ParseInitNodeDeclaration(syntaxText);

            yield return CreateSymbolQuickInfoControl(syntax, ImageMonikers.FromSymbol(initNodeSymbol));
        }

        public override IEnumerable<object> VisitInitNodeAliasSymbol(IInitNodeAliasSymbol initNodeAliasSymbol) {
            return Visit(initNodeAliasSymbol.InitNode);
        }

        public override IEnumerable<object> VisitExitNodeSymbol(IExitNodeSymbol exitNodeSymbol) {

            var syntaxText = $"{exitNodeSymbol.Syntax.ExitKeyword} {exitNodeSymbol.Syntax.Identifier}";
            var syntax     = Syntax.ParseExitNodeDeclaration(syntaxText);

            yield return CreateSymbolQuickInfoControl(syntax, ImageMonikers.FromSymbol(exitNodeSymbol));
        }

        public override IEnumerable<object> VisitEndNodeSymbol(IEndNodeSymbol endNodeSymbol) {

            var syntaxText = endNodeSymbol.Name;
            var syntax     = Syntax.ParseEndNodeDeclaration(syntaxText);

            yield return CreateSymbolQuickInfoControl(syntax, ImageMonikers.FromSymbol(endNodeSymbol));
        }

        public override IEnumerable<object> VisitTaskNodeSymbol(ITaskNodeSymbol taskNodeSymbol) {

            var alias = taskNodeSymbol.Syntax.IdentifierAlias.IsMissing ? String.Empty: taskNodeSymbol.Syntax.IdentifierAlias.ToString();

            var syntaxText = $"{taskNodeSymbol.Syntax.TaskKeyword} {taskNodeSymbol.Syntax.Identifier} {alias}";
            var syntax     = Syntax.ParseTaskNodeDeclaration(syntaxText);

            yield return CreateSymbolQuickInfoControl(syntax, ImageMonikers.FromSymbol(taskNodeSymbol));
        }

        public override IEnumerable<object> VisitTaskNodeAliasSymbol(ITaskNodeAliasSymbol taskNodeAlias) {
            return Visit(taskNodeAlias.TaskNode);
        }

        public override IEnumerable<object> VisitChoiceNodeSymbol(IChoiceNodeSymbol choiceNodeSymbol) {

            var syntaxText = $"{choiceNodeSymbol.Syntax.ChoiceKeyword} {choiceNodeSymbol.Syntax.Identifier}";
            var syntax     = Syntax.ParseChoiceNodeDeclaration(syntaxText);

            yield return CreateSymbolQuickInfoControl(syntax, ImageMonikers.FromSymbol(choiceNodeSymbol));
        }
        
        public override IEnumerable<object> VisitViewNodeSymbol(IViewNodeSymbol viewNodeSymbol) {

            var syntaxText = $"{viewNodeSymbol.Syntax.ViewKeyword} {viewNodeSymbol.Syntax.Identifier}";
            var syntax     = Syntax.ParseViewNodeDeclaration(syntaxText);

            yield return CreateSymbolQuickInfoControl(syntax, ImageMonikers.FromSymbol(viewNodeSymbol));
        }
        
        public override IEnumerable<object> VisitDialogNodeSymbol(IDialogNodeSymbol dialogNodeSymbol) {

            var syntaxText = $"{dialogNodeSymbol.Syntax.DialogKeyword} {dialogNodeSymbol.Syntax.Identifier}";
            var syntax = Syntax.ParseDialogNodeDeclaration(syntaxText);

            yield return CreateSymbolQuickInfoControl(syntax, ImageMonikers.FromSymbol(dialogNodeSymbol));
        }

        #endregion

        public override IEnumerable<object> VisitSignalTriggerSymbol(ISignalTriggerSymbol signalTriggerSymbol) {

            var signalTriggerCodeModel = SignalTriggerCodeModel.FromSignalTrigger(signalTriggerSymbol);
            
            StackPanel panel = new StackPanel {
                Orientation = Orientation.Vertical
            };

            var control = new SymbolQuickInfoControl();
            control.CrispImage.Moniker  = ImageMonikers.FromSymbol(signalTriggerSymbol);
            control.TextContent.Content = SyntaxQuickinfoBuilderService.ToTextBlock(signalTriggerCodeModel);

            panel.Children.Add(control);

            yield return panel;
        }
    }
}