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

        // TODO To FullPath
        public ITaskItem[] Files { get; set; }


        protected override string GenerateFullPathToTool() {
            return Path.Combine(Path.GetDirectoryName(GetType().Assembly.Location), ToolName);
        }

        protected override string GenerateCommandLineCommands() {


            StringBuilder sb = new StringBuilder();

            sb.Append("-g ");
            sb.Append($"-s:");
            foreach (var file in Files) {
                sb.Append($"\"{file.GetMetadata("FullPath")}\" ");
            }

        //    CommandLineBuilder clb = new CommandLineBuilder();
        //
        //    clb.AppendSwitch("-g");
        //   clb.AppendFileNamesIfNotNull(Files, "-s:");
            return sb.ToString();
        }

        protected override string ToolName => "nav.exe";

    }
}