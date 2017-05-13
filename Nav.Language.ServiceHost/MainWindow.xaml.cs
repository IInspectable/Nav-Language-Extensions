using System.Windows;

namespace Nav.Language.ServiceHost {

    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();

            Title      = ThisAssembly.ProductName; // TODO Title mit Uri?
            Visibility = Visibility.Visible;
            Visibility = Visibility.Hidden;
        }
    }
}