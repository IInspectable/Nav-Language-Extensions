using System.Windows;

namespace Nav.Language.ServiceHost {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();

            Title = "Nav Language Host Service"; // TODO Title mit Uri?
            Visibility = Visibility.Visible;
            Visibility = Visibility.Hidden;
        }
    }
}