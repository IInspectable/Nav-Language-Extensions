#region Using Directives


#endregion

using System.Windows;

namespace Pharmatechnik.Nav.Language.Extension.Options {

    public partial class AdvancedOptionsControl {
        public AdvancedOptionsControl() {
            InitializeComponent();
        }

        void HighlightReferencesUnderCursor_OnChecked(object sender, RoutedEventArgs e) {

            HighlightReferencesUnderInclude.IsEnabled = HighlightReferencesUnderCursor.IsChecked.GetValueOrDefault();
        }
    }
}
