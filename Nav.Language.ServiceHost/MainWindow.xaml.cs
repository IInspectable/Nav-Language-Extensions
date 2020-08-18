using System.Windows;

namespace Nav.Language.ServiceHost {

    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();

            Title      = MyAssembly.ProductName; // TODO Title mit Uri?
            Visibility = Visibility.Visible;
            Visibility = Visibility.Hidden;
        }
    }
}