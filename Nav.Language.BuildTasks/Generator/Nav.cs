#region Using Directives

using System.IO;
using System.Text;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

#endregion

namespace Pharmatechnik.Nav.Language.BuildTasks {

    public class Nav: ToolTask {

        public bool Force { get; set; }
        public bool GenerateToClasses { get; set; }
        public bool UseSyntaxCache { get; set; }
        public ITaskItem[] Sources { get; set; }
        
        protected override string GenerateFullPathToTool() {
            // ReSharper disable once AssignNullToNotNullAttribute
            return Path.Combine(Path.GetDirectoryName(GetType().Assembly.Location), ToolName);
        }

        protected override Encoding ResponseFileEncoding => Encoding.UTF8;

        protected override string GenerateResponseFileCommands() {

            var clb = new CommandLineBuilder();

            if (GenerateToClasses) {
                clb.AppendSwitch("-g");
            }

            if (Force) {
                clb.AppendSwitch("-f");
            }

            if (UseSyntaxCache) {
                clb.AppendSwitch("-c");
            }
            
            // TODO Verbosity aus Task?
            clb.AppendSwitch("-v");              
            
            clb.AppendSwitch("-s");
            clb.AppendFileNamesIfNotNull(Sources, " ");

            return clb.ToString();
        }

        protected override string ToolName => "nav.exe";
        const string InfoPrefix = "Info:";
        const string VerbosePrefix = "Verbose:";
        protected override void LogEventsFromTextOutput(string singleLine, MessageImportance messageImportance) {
            
            if (singleLine.StartsWith(InfoPrefix)) {
                messageImportance = MessageImportance.High;
                singleLine = singleLine.Substring(InfoPrefix.Length);
            } else if (singleLine.StartsWith(VerbosePrefix)) {
                messageImportance = MessageImportance.Low;
                singleLine = singleLine.Substring(VerbosePrefix.Length);
            }

            base.LogEventsFromTextOutput(singleLine, messageImportance);            
        }
    }
}