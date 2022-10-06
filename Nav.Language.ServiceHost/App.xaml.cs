#region Using Directives

using System;
using System.Windows;
using System.Diagnostics;
using System.Net.Security;
using System.ServiceModel;
using System.Threading;

using Nav.Language.Service;

using Pharmatechnik.Nav.Utilities.Logging;

#endregion

namespace Nav.Language.ServiceHost; 

public partial class App : Application {

    static readonly Logger Logger = Logger.Create<App>();

    void OnStartup(object sender, StartupEventArgs e) {

        var scp = ServiceConnectionParams.FromCommandLine(e.Args);
        if(scp == null) {
            Logger.Error("Missing service connection parameter. Terminating Service Host.");
            Terminate(1);
            return;
        }

        // Step 1
        Logger.Debug("Create the NetNamedPipeBinding.");
            
        var binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
        binding.ReceiveTimeout                     = TimeSpan.MaxValue;
        binding.SendTimeout                        = TimeSpan.MaxValue;
        binding.Security.Transport.ProtectionLevel = ProtectionLevel.None;
        binding.MaxReceivedMessageSize             = scp.BindingMaxReceivedMessageSize;

        // Step 3
        Logger.Debug("Create the service hosts.");

        // Step 4
        Logger.Debug("Signal parent process that host is ready so that it can proceed.");

        var readyEvent = new EventWaitHandle(false, EventResetMode.ManualReset, scp.ReadyEventName);
        readyEvent.Set();
        readyEvent.Close();

        try {
            // Step 5
            Logger.Debug($"Get the parent process {scp.ParentProcessId}.");
            Process parentProcess = Process.GetProcessById(scp.ParentProcessId);
            parentProcess.EnableRaisingEvents =  true;
            parentProcess.Exited              += (_, __) => Terminate(0);

        } catch(Exception ex) {
            Logger.Error(ex, $"Error getting the owner process {scp.ParentProcessId}.");
        }
    }

    void Terminate(int exitCode) {
        Environment.Exit(exitCode);
    }
}