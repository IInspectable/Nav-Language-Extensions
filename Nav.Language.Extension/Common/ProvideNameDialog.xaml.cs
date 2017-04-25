#region Using Directives

using System.Windows;
using Microsoft.VisualStudio.Imaging.Interop;
using Microsoft.VisualStudio.PlatformUI;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Common {

    // TODO Als Service abieten
    partial class ProvideNameDialog : DialogWindow {
        public ProvideNameDialog() {
            InitializeComponent();
        }
     
        void OnOkClick(object sender, RoutedEventArgs e) {
            DialogResult = true;
        }

        void OnCancelClick(object sender, RoutedEventArgs e) {
            DialogResult = false;
        }

        public ImageMoniker ImageMoniker {
            get { return CrispImage.Moniker; }
            set { CrispImage.Moniker = value; }
        }

        public string InputString {
            get { return InputText.Text; }
            set { InputText.Text = value; }
        }

        public string OK {
            get { return "OK"; }
        }

        public string Cancel {
            get { return "Cancel"; }
        }
    }
}
