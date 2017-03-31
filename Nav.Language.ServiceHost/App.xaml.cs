using System.Windows;
using Nav.Language.Service;

namespace Nav.Language.ServiceHost {

    public partial class App : Application {

        private void OnStartup(object sender, StartupEventArgs e) {

            ServiceConnectionArgs cla = ServiceConnectionArgs.FromCommandLine(e.Args);
            if (cla == null) {
                Shutdown(1);
                return;
            }
            // TODO Logging
            // TODO implement  OnStartup   
        }
    }
}