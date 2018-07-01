#region Using Directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

using Microsoft.VisualStudio.Imaging.Interop;

using Pharmatechnik.Nav.Language.CodeGen;
using Pharmatechnik.Nav.Language.Extension.Images;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.QuickInfo {

    sealed class SymbolQuickInfoBuilder: SymbolVisitor<object> {

        #region Infrastructure

        SymbolQuickInfoBuilder(ISymbol originatingSymbol, SyntaxQuickinfoBuilderService syntaxQuickinfoBuilderService) {
            OriginatingSymbol             = originatingSymbol;
            SyntaxQuickinfoBuilderService = syntaxQuickinfoBuilderService;
        }

        ISymbol                       OriginatingSymbol             { get; }
        SyntaxQuickinfoBuilderService SyntaxQuickinfoBuilderService { get; }

        public static object Build(ISymbol source, SyntaxQuickinfoBuilderService syntaxQuickinfoBuilderService) {
            var builder = new SymbolQuickInfoBuilder(source, syntaxQuickinfoBuilderService);
            return builder.Visit(source);
        }

        SymbolQuickInfoControl CreateSymbolQuickInfoControl(SyntaxNode syntax, ImageMoniker imageMoniker) {

            var control = new SymbolQuickInfoControl {
                CrispImage  = {Moniker = imageMoniker},
                TextContent = {Content = SyntaxQuickinfoBuilderService.ToTextBlock(syntax.SyntaxTree)}
            };
            return control;
        }

        #endregion

        #region ConnectionPoints

        public override object VisitConnectionPointReferenceSymbol(IConnectionPointReferenceSymbol connectionPointReferenceSymbol) {
            if (connectionPointReferenceSymbol.Declaration == null) {
                return null;
            }

            return Visit(connectionPointReferenceSymbol.Declaration);
        }

        public override object VisitInitConnectionPointSymbol(IInitConnectionPointSymbol initConnectionPointSymbol) {

            var syntaxText = $"{initConnectionPointSymbol.Syntax.InitKeyword} {initConnectionPointSymbol.Syntax.Identifier}";
            var syntax     = Syntax.ParseInitNodeDeclaration(syntaxText);

            return CreateSymbolQuickInfoControl(syntax, ImageMonikers.FromSymbol(initConnectionPointSymbol));
        }

        public override object VisitExitConnectionPointSymbol(IExitConnectionPointSymbol exitConnectionPointSymbol) {

            var syntaxText = $"{exitConnectionPointSymbol.Syntax.ExitKeyword} {exitConnectionPointSymbol.Syntax.Identifier}";
            var syntax     = Syntax.ParseExitNodeDeclaration(syntaxText);

            return CreateSymbolQuickInfoControl(syntax, ImageMonikers.FromSymbol(exitConnectionPointSymbol));
        }

        public override object VisitEndConnectionPointSymbol(IEndConnectionPointSymbol endConnectionPointSymbol) {

            var syntaxText = endConnectionPointSymbol.Name;
            var syntax     = Syntax.ParseEndNodeDeclaration(syntaxText);

            return CreateSymbolQuickInfoControl(syntax, ImageMonikers.FromSymbol(endConnectionPointSymbol));
        }

        #endregion

        public override object VisitTaskDeclarationSymbol(ITaskDeclarationSymbol taskDeclarationSymbol) {

            var syntaxText = $"taskref {taskDeclarationSymbol.Name}";
            var syntax     = Syntax.ParseTaskDeclaration(syntaxText);

            return CreateSymbolQuickInfoControl(syntax, ImageMonikers.FromSymbol(taskDeclarationSymbol));
        }

        public override object VisitTaskDefinitionSymbol(ITaskDefinitionSymbol taskDefinitionSymbol) {

            var syntaxText = $"{taskDefinitionSymbol.Syntax.TaskKeyword} {taskDefinitionSymbol.Name}";
            var syntax     = Syntax.ParseTaskDefinition(syntaxText);

            return CreateSymbolQuickInfoControl(syntax, ImageMonikers.FromSymbol(taskDefinitionSymbol));
        }

        public override object VisitIncludeSymbol(IIncludeSymbol includeSymbol) {

            StackPanel panel = new StackPanel {
                Orientation = Orientation.Vertical
            };

            var control = new SymbolQuickInfoControl {
                CrispImage  = {Moniker = ImageMonikers.FromSymbol(includeSymbol)},
                TextContent = {Content = SyntaxQuickinfoBuilderService.ToTextBlock(includeSymbol.FileName, SyntaxTokenClassification.Identifier)}
            };

            panel.Children.Add(control);

            //foreach(var taskDecl in includeSymbol.TaskDeklarations) {
            //    foreach(var elem in Visit(taskDecl).OfType<SymbolQuickInfoControl>()) {
            //        elem.Margin = new Thickness(20,0,0,0);
            //        panel.Children.Add(elem);
            //    }
            //}

            return panel;
        }

        #region Nodes

        public override object VisitNodeReferenceSymbol(INodeReferenceSymbol nodeReferenceSymbol) {
            if (nodeReferenceSymbol.Declaration == null) {
                return null;
            }

            return Visit(nodeReferenceSymbol.Declaration);
        }

        public override object VisitInitNodeSymbol(IInitNodeSymbol initNodeSymbol) {
            // Wir zeigen keinen Tooltip für das init Keyword an, wenn es einen Alias gibt
            if (OriginatingSymbol == initNodeSymbol && initNodeSymbol.Alias != null) {
                return null;
            }

            var syntaxText = $"{initNodeSymbol.Syntax.InitKeyword} {initNodeSymbol.Syntax.Identifier}";
            var syntax     = Syntax.ParseInitNodeDeclaration(syntaxText);

            return CreateSymbolQuickInfoControl(syntax, ImageMonikers.FromSymbol(initNodeSymbol));
        }

        public override object VisitInitNodeAliasSymbol(IInitNodeAliasSymbol initNodeAliasSymbol) {
            return Visit(initNodeAliasSymbol.InitNode);
        }

        public override object VisitExitNodeSymbol(IExitNodeSymbol exitNodeSymbol) {

            var syntaxText = $"{exitNodeSymbol.Syntax.ExitKeyword} {exitNodeSymbol.Syntax.Identifier}";
            var syntax     = Syntax.ParseExitNodeDeclaration(syntaxText);

            return CreateSymbolQuickInfoControl(syntax, ImageMonikers.FromSymbol(exitNodeSymbol));
        }

        public override object VisitEndNodeSymbol(IEndNodeSymbol endNodeSymbol) {

            var syntaxText = endNodeSymbol.Name;
            var syntax     = Syntax.ParseEndNodeDeclaration(syntaxText);

            return CreateSymbolQuickInfoControl(syntax, ImageMonikers.FromSymbol(endNodeSymbol));
        }

        public override object VisitTaskNodeSymbol(ITaskNodeSymbol taskNodeSymbol) {

            var alias = taskNodeSymbol.Syntax.IdentifierAlias.IsMissing ? String.Empty : taskNodeSymbol.Syntax.IdentifierAlias.ToString();

            var syntaxText = $"{taskNodeSymbol.Syntax.TaskKeyword} {taskNodeSymbol.Syntax.Identifier} {alias}";
            var syntax     = Syntax.ParseTaskNodeDeclaration(syntaxText);

            return CreateSymbolQuickInfoControl(syntax, ImageMonikers.FromSymbol(taskNodeSymbol));
        }

        public override object VisitTaskNodeAliasSymbol(ITaskNodeAliasSymbol taskNodeAlias) {
            return Visit(taskNodeAlias.TaskNode);
        }

        public override object VisitChoiceNodeSymbol(IChoiceNodeSymbol choiceNodeSymbol) {

            var syntaxText = $"{choiceNodeSymbol.Syntax.ChoiceKeyword} {choiceNodeSymbol.Syntax.Identifier}";
            var syntax     = Syntax.ParseChoiceNodeDeclaration(syntaxText);

            return CreateSymbolQuickInfoControl(syntax, ImageMonikers.FromSymbol(choiceNodeSymbol));
        }

        public override object VisitViewNodeSymbol(IViewNodeSymbol viewNodeSymbol) {

            var syntaxText = $"{viewNodeSymbol.Syntax.ViewKeyword} {viewNodeSymbol.Syntax.Identifier}";
            var syntax     = Syntax.ParseViewNodeDeclaration(syntaxText);

            return CreateSymbolQuickInfoControl(syntax, ImageMonikers.FromSymbol(viewNodeSymbol));
        }

        public override object VisitDialogNodeSymbol(IDialogNodeSymbol dialogNodeSymbol) {

            var syntaxText = $"{dialogNodeSymbol.Syntax.DialogKeyword} {dialogNodeSymbol.Syntax.Identifier}";
            var syntax     = Syntax.ParseDialogNodeDeclaration(syntaxText);

            return CreateSymbolQuickInfoControl(syntax, ImageMonikers.FromSymbol(dialogNodeSymbol));
        }

        #endregion

        public override object VisitSignalTriggerSymbol(ISignalTriggerSymbol signalTriggerSymbol) {

            var signalTriggerCodeModel = SignalTriggerCodeInfo.FromSignalTrigger(signalTriggerSymbol);

            StackPanel panel = new StackPanel {
                Orientation = Orientation.Vertical
            };

            var control = new SymbolQuickInfoControl {
                CrispImage  = {Moniker = ImageMonikers.FromSymbol(signalTriggerSymbol)},
                TextContent = {Content = SyntaxQuickinfoBuilderService.ToTextBlock(signalTriggerCodeModel)}
            };

            panel.Children.Add(control);

            return panel;
        }

        public override object VisitEdgeModeSymbol(IEdgeModeSymbol edgeModeSymbol) {

            var edgeViewModel = new EdgeViewModel(
                moniker: ImageMonikers.Edge,
                calls: edgeModeSymbol.Edge
                                     .GetReachableCalls()
                                     .OrderBy(call => call.Node.Name)
                                     .Select(call => new CallViewModel(
                                                 edgeModeMoniker: ImageMonikers.FromSymbol(call.EdgeMode),
                                                 verb: SyntaxQuickinfoBuilderService.ToTextBlock(GetVerb(call.EdgeMode), SyntaxTokenClassification.Keyword),
                                                 nodeMoniker: ImageMonikers.FromSymbol(call.Node),
                                                 node: SyntaxQuickinfoBuilderService.ToTextBlock(call.Node.Name, SyntaxTokenClassification.Identifier
                                                 ))));

            var control = new EdgeQuickInfoControl {
                DataContext = edgeViewModel
            };

            return control;
        }

        string GetVerb(IEdgeModeSymbol callEdgeMode) {
            switch (callEdgeMode.EdgeMode) {

                case EdgeMode.Modal:
                    return "modal";
                case EdgeMode.NonModal:
                    return "non-modal";
                case EdgeMode.Goto:
                    return "go to";
                default:
                    return "";
            }
        }

    }

    class CallViewModel {

        public CallViewModel(ImageMoniker edgeModeMoniker, object verb, ImageMoniker nodeMoniker, object node) {
            EdgeModeMoniker = edgeModeMoniker;
            Verb            = verb;
            NodeMoniker     = nodeMoniker;
            Node            = node;
        }

        public ImageMoniker EdgeModeMoniker { get; }
        public object       Verb            { get; }
        public ImageMoniker NodeMoniker     { get; }
        public object       Node            { get; }

    }

    class EdgeViewModel {

        public EdgeViewModel(ImageMoniker moniker, IEnumerable<CallViewModel> calls) {
            Moniker = moniker;
            Calls   = new List<CallViewModel>(calls);
        }

        public ImageMoniker                 Moniker { get; }
        public IReadOnlyList<CallViewModel> Calls   { get; }

    }

}