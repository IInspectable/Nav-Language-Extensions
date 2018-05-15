#region Using Directives

using System;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Common {
    static class ShellUtil {

        public static void ShowInfoMessage(string message) {
            ShowMessagebox(message, OLEMSGICON.OLEMSGICON_INFO);
        }

        public static void ShowErrorMessage(string message) {      
            ThreadHelper.ThrowIfNotOnUIThread();
            ShowMessagebox(message, OLEMSGICON.OLEMSGICON_CRITICAL);
        }

         static void ShowMessagebox(string message, OLEMSGICON msgicon, 
                                    OLEMSGBUTTON msgbtn = OLEMSGBUTTON.OLEMSGBUTTON_OK, 
                                    OLEMSGDEFBUTTON msgdefbtn = OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST) {

            ThreadHelper.ThrowIfNotOnUIThread();

            var uiShell = (IVsUIShell) ServiceProvider.GlobalProvider.GetService(typeof(SVsUIShell));
            var unused  = Guid.Empty;

            uiShell.ShowMessageBox(
                dwCompRole     : 0,
                rclsidComp     : ref unused,
                pszTitle       : null,
                pszText        : message,
                pszHelpFile    : null,
                dwHelpContextID: 0,
                msgbtn         : msgbtn,
                msgdefbtn      : msgdefbtn,
                msgicon        : msgicon,
                fSysAlert      : 0,
                // ReSharper disable once UnusedVariable
                pnResult       : out var result);
        }
    }
}
