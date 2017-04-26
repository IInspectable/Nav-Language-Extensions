#region Using Directives

using System.Windows;
using Microsoft.VisualStudio.PlatformUI;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Common {
    
    partial class InputDialog : DialogWindow {

        readonly InputDialogViewModel _viewModel;

        public InputDialog(InputDialogViewModel viewModel) {
            _viewModel = viewModel;
            InitializeComponent();
            Loaded += (o, e)=> InputText.SelectAll();
            DataContext = _viewModel;
        }
        
        void OnOkClick(object sender, RoutedEventArgs e) {
            if (_viewModel.HasErrors) {
                // TODO Button Deaktivieren, wenn Fehler vorhanden
                return;
            }
            DialogResult = true;
        }

        void OnCancelClick(object sender, RoutedEventArgs e) {
            DialogResult = false;
        }
    }
}
