#region Using Directives

using System;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Pharmatechnik.Nav.Language.CodeGen;

#endregion

namespace Pharmatechnik.Nav.Language.BuildTasks {

    public class Nav: ToolTask {

        public bool Force { get; set; }
        public bool GenerateToClasses { get; set; }
        public bool UseSyntaxCache { get; set; }

        // TOO FullPath, wenn von Commandozeile aus gestartet?
        public ITaskItem[] Sources { get; set; }
        
        protected override string GenerateFullPathToTool() {
            return Path.Combine(Path.GetDirectoryName(GetType().Assembly.Location), ToolName);
        }

        protected override string GenerateCommandLineCommands() {
            var clb = new CommandLineBuilder();
            
            clb.AppendSwitch("-g");
            if (GenerateToClasses) {
                clb.AppendSwitch("-g");
            }
            if (Force) {
                clb.AppendSwitch("-f");
            }
            if (UseSyntaxCache) {
                clb.AppendSwitch("-c");
            }
            clb.AppendSwitch("-s");
            clb.AppendFileNamesIfNotNull(Sources, " ");
         
            return clb.ToString();
        }

        protected override string ToolName => "nav.exe";
    }
}