#region Using Directives

using System;
using System.IO;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Pharmatechnik.Nav.Language.Logging;

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

        protected override string GenerateResponseFileCommands() {
            var clb = new CommandLineBuilder();
            clb.AppendFileNamesIfNotNull(Sources, " ");
            return clb.ToString();
        }

        protected override string GenerateCommandLineCommands() {

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
            clb.AppendSwitch("-v");
         
            return clb.ToString();
        }

        protected override string ToolName => "nav.exe";

        protected override void LogEventsFromTextOutput(string singleLine, MessageImportance messageImportance) {

            if (singleLine.StartsWith(ConsoleLogger.InfoPrefix)) {
                messageImportance = MessageImportance.High;
                singleLine = singleLine.Substring(ConsoleLogger.InfoPrefix.Length);
            } else if (singleLine.StartsWith(ConsoleLogger.VerbosePrefix)) {
                messageImportance = MessageImportance.Low;
                singleLine = singleLine.Substring(ConsoleLogger.VerbosePrefix.Length);
            }

            base.LogEventsFromTextOutput(singleLine, messageImportance);            
        }
    }
}