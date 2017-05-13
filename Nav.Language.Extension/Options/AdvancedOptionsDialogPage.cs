#region Using Directives

using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;

using Microsoft.VisualStudio.Shell;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Options {
    // TODO Default Settings?
    [Guid("2D380106-1E1D-489C-B0AE-707041CB79A0")]
    class AdvancedOptionsDialogPage : UIElementDialogPage, IAdvancedOptions {

        AdvancedOptionsControl _advancedOptionsControl;
        bool _highlightReferencesUnderInclude;

        public AdvancedOptionsDialogPage() {
            SemanticHighlighting = true;
            HighlightReferencesUnderCursor = true;            
        }

        public const string PageName = "Advanced";

        protected override UIElement Child {
            get {
                return _advancedOptionsControl ?? (_advancedOptionsControl = new AdvancedOptionsControl());
            }
        }

        public bool SemanticHighlighting { get; set; }
        public bool HighlightReferencesUnderCursor { get; set; }
        public bool HighlightReferencesUnderInclude {
            get { return _highlightReferencesUnderInclude && HighlightReferencesUnderCursor; }
            set { _highlightReferencesUnderInclude = value; }
        }

        protected override void OnActivate(CancelEventArgs e) {
            base.OnActivate(e);
            _advancedOptionsControl.SemanticHighlighting.IsChecked            = SemanticHighlighting;
            _advancedOptionsControl.HighlightReferencesUnderCursor.IsChecked  = HighlightReferencesUnderCursor;
            _advancedOptionsControl.HighlightReferencesUnderInclude.IsChecked = HighlightReferencesUnderInclude;
        }

        protected override void OnApply(PageApplyEventArgs args) {
            if (args.ApplyBehavior == ApplyKind.Apply) {
                SemanticHighlighting            = _advancedOptionsControl.SemanticHighlighting.IsChecked.GetValueOrDefault();
                HighlightReferencesUnderCursor  = _advancedOptionsControl.HighlightReferencesUnderCursor.IsChecked.GetValueOrDefault();
                HighlightReferencesUnderInclude = _advancedOptionsControl.HighlightReferencesUnderInclude.IsChecked.GetValueOrDefault();
            }

            base.OnApply(args);
        }
    }
}