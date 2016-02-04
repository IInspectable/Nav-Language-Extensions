﻿#region Using Directives

using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Diagnostics {

    [Export(typeof(IWpfTextViewMarginProvider))]
    [Name(DiagnosticStripeMargin.MarginName)]

    [Order(After = PredefinedMarginNames.OverviewChangeTracking)]
    [Order(After = PredefinedMarginNames.OverviewMark)]
    [Order(After = PredefinedMarginNames.OverviewError)]
    [Order(After = PredefinedMarginNames.OverviewSourceImage)]

    [MarginContainer(PredefinedMarginNames.VerticalScrollBar)]

    [ContentType(NavLanguageContentDefinitions.ContentType)]
    [TextViewRole(PredefinedTextViewRoles.Interactive)]
    sealed class DiagnosticStripeMarginProvider : IWpfTextViewMarginProvider {

        readonly IEditorFormatMapService _editorFormatMapService;

        [ImportingConstructor]
        public DiagnosticStripeMarginProvider(IEditorFormatMapService editorFormatMapService) {
            _editorFormatMapService = editorFormatMapService;
        }

        #region Documentation
        /// <summary>
        /// Creates an <see cref="IWpfTextViewMargin"/> for the given <see cref="IWpfTextViewHost"/>.
        /// </summary>
        /// <param name="wpfTextViewHost">The <see cref="IWpfTextViewHost"/> for which to create the <see cref="IWpfTextViewMargin"/>.</param>
        /// <param name="marginContainer">The margin that will contain the newly-created margin.</param>
        /// <returns>The <see cref="IWpfTextViewMargin"/>.
        /// The value may be null if this <see cref="IWpfTextViewMarginProvider"/> does not participate for this context.
        /// </returns>
        #endregion
        public IWpfTextViewMargin CreateMargin(IWpfTextViewHost wpfTextViewHost, IWpfTextViewMargin marginContainer) {

            var scrollBar = marginContainer.GetTextViewMargin(PredefinedMarginNames.VerticalScrollBar) as IVerticalScrollBar;
            if (scrollBar == null) {
                return null;
            }

            return new DiagnosticStripeMargin(
                wpfTextViewHost.TextView,                  
                scrollBar, 
                _editorFormatMapService);
        }
    }
}