#region Using Directives

using System;
using System.Windows;
using System.Windows.Controls;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.FindReferences {

    partial class TableEntriesSnapshot {

        class LazyTooltip {

            private readonly FrameworkElement _element;

            private readonly Func<ToolTip> _createToolTip;

            private LazyTooltip(FrameworkElement element,
                                Func<ToolTip> createToolTip) {
                _element       = element;
                _createToolTip = createToolTip;

                // Set ourselves as the tooltip of this text block.  This will let WPF know that 
                // it should attempt to show tooltips here.  When WPF wants to show the tooltip 
                // though we'll hear about it "ToolTipOpening".  When that happens, we'll swap
                // out ourselves with a real tooltip that is lazily created.  When that tooltip
                // is the dismissed, we'll release the resources associated with it and we'll
                // reattach ourselves.
                _element.ToolTip = this;

                element.ToolTipOpening += OnToolTipOpening;
                element.ToolTipClosing += OnToolTipClosing;

            }

            public static void AttachTo(FrameworkElement element, Func<ToolTip> createToolTip) {
                var _ = new LazyTooltip(element, createToolTip);
            }

            private void OnToolTipOpening(object sender, ToolTipEventArgs e) {
                _element.ToolTip = _createToolTip();
            }

            private void OnToolTipClosing(object sender, ToolTipEventArgs e) {
                _element.ToolTip = this;

            }

        }

    }

}