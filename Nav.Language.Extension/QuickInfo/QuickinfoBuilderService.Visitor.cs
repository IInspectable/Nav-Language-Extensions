#region Using Directives

using System.Collections.Generic;
using System.Linq;
using System.Windows;

using JetBrains.Annotations;

using Microsoft.VisualStudio.Imaging.Interop;

using Pharmatechnik.Nav.Language.Extension.Images;
using Pharmatechnik.Nav.Language.Text;

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

            public override UIElement VisitEdgeModeSymbol(IEdgeModeSymbol edgeModeSymbol) {

                var edgeViewModel = new EdgeViewModel(
                    moniker: ImageMonikers.Edge,
                    calls: edgeModeSymbol.Edge
                                         .GetReachableCalls()
                                         .OrderBy(call => call.Node.Name)
                                         .Select(call => new CallViewModel(
                                                     edgeModeMoniker: ImageMonikers.FromSymbol(call.EdgeMode),
                                                     verb: QuickinfoBuilderService.ToTextBlock(call.EdgeMode.Verb, TextClassification.Keyword),
                                                     nodeMoniker: ImageMonikers.FromSymbol(call.Node),
                                                     node: QuickinfoBuilderService.ToTextBlock(call.Node.Name, TextClassification.Identifier
                                                     ))));

                var control = new EdgeQuickInfoControl {
                    DataContext = edgeViewModel
                };

                return control;
            }

        }

        class CallViewModel {

            public CallViewModel(ImageMoniker edgeModeMoniker, object verb, ImageMoniker nodeMoniker, object node) {
                EdgeModeMoniker = edgeModeMoniker;
                Verb            = verb;
                NodeMoniker     = nodeMoniker;
                Node            = node;
            }

            [UsedImplicitly]
            public ImageMoniker EdgeModeMoniker { get; }

            [UsedImplicitly]
            public object Verb { get; }

            [UsedImplicitly]
            public ImageMoniker NodeMoniker { get; }

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