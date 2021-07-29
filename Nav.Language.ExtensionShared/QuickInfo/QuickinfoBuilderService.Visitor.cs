#region Using Directives

using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using JetBrains.Annotations;

using Microsoft.VisualStudio.Imaging.Interop;

using Pharmatechnik.Nav.Language.Extension.Images;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.QuickInfo {

    partial class QuickinfoBuilderService {

        sealed class SymbolQuickInfoVisitor: SymbolVisitor<UIElement> {

            #region Infrastructure

            SymbolQuickInfoVisitor(ISymbol originatingSymbol, QuickinfoBuilderService quickinfoBuilderService) {
                OriginatingSymbol       = originatingSymbol;
                QuickinfoBuilderService = quickinfoBuilderService;
            }

            ISymbol                 OriginatingSymbol       { get; }
            QuickinfoBuilderService QuickinfoBuilderService { get; }

            [CanBeNull]
            public static UIElement Build(ISymbol source, QuickinfoBuilderService quickinfoBuilderService) {
                var builder = new SymbolQuickInfoVisitor(source, quickinfoBuilderService);
                return builder.Visit(source);
            }

            #endregion

            protected override UIElement DefaultVisit(ISymbol symbol) {
                return QuickinfoBuilderService.CreateDefaultSymbolQuickInfoControl(symbol);
            }

            public override UIElement VisitInitNodeSymbol(IInitNodeSymbol initNodeSymbol) {
                // Wir zeigen keinen Tooltip für das init Keyword an, wenn es einen Alias gibt
                if (OriginatingSymbol == initNodeSymbol && initNodeSymbol.Alias != null) {
                    return null;
                }

                return DefaultVisit(initNodeSymbol);

            }

            public override UIElement VisitChoiceNodeSymbol(IChoiceNodeSymbol choiceNodeSymbol) {
              
                var choiceNodeRef=choiceNodeSymbol.References.FirstOrDefault() as IChoiceNodeReferenceSymbol;
                if(choiceNodeRef != null) {
                    return VisitChoiceNodeReferenceSymbol(choiceNodeRef);
                }

                return base.VisitChoiceNodeSymbol(choiceNodeSymbol);
            }

            public override UIElement VisitChoiceNodeReferenceSymbol(IChoiceNodeReferenceSymbol choiceNodeReferenceSymbol) {
                
                // Node
                //    Call 1 
                //    Call 2
                //    ...
                var node  = base.VisitChoiceNodeReferenceSymbol(choiceNodeReferenceSymbol);
                var calls = VisitEdgeModeSymbol(choiceNodeReferenceSymbol.Edge.EdgeMode);
               
                var panel = new StackPanel();
                panel.Children.Add(node);
                panel.Children.Add(calls);
                panel.Orientation = Orientation.Vertical;

                return panel;
            }

            public override UIElement VisitEdgeModeSymbol(IEdgeModeSymbol edgeModeSymbol) {

                var edgeViewModel = new EdgeViewModel(
                    moniker: ImageMonikers.Edge,
                    calls: edgeModeSymbol.Edge
                                         .GetReachableCalls()
                                         .OrderBy(call => call.Node.Name)
                                         .Select(call => new CallViewModel(
                                                     edgeModeMoniker: ImageMonikers.FromSymbol(call.EdgeMode),
                                                     node: Visit(call.Node)
                                                 )));

                var control = new EdgeQuickInfoControl {
                    DataContext = edgeViewModel
                };

                return control;
            }

        }

        class CallViewModel {

            public CallViewModel(ImageMoniker edgeModeMoniker, object node) {
                EdgeModeMoniker = edgeModeMoniker;

                Node = node;
            }

            [UsedImplicitly]
            public ImageMoniker EdgeModeMoniker { get; }

            [UsedImplicitly]
            public object Node { get; }

        }

        class EdgeViewModel {

            public EdgeViewModel(ImageMoniker moniker, IEnumerable<CallViewModel> calls) {
                Moniker = moniker;
                Calls   = new List<CallViewModel>(calls);
            }

            [UsedImplicitly]
            public ImageMoniker Moniker { get; }

            [UsedImplicitly]
            public IReadOnlyList<CallViewModel> Calls { get; }

        }

    }

}