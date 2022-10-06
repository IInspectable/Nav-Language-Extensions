#region Using Directives

using Microsoft.Build.Utilities;

#endregion

namespace Pharmatechnik.Nav.Language.BuildTasks; 

static class CommandLineBuilderExtensions {

    public static void AppendSwitchIfPresent(this CommandLineBuilder commandLineBuilder, bool switchValue, string switchName) {
        if(switchValue) {
            commandLineBuilder.AppendSwitch(switchName);
        }            
    }
}