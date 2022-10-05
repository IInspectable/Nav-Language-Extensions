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
        public bool FullPaths { get; set; }

        public ITaskItem   ProjectRootDirectory { get; set; }
        public ITaskItem   IwflRootDirectory    { get; set; }
        public ITaskItem[] Sources              { get; set; }
        
        protected override string GenerateFullPathToTool() {
            // ReSharper disable once AssignNullToNotNullAttribute
            return Path.Combine(Path.GetDirectoryName(GetType().Assembly.Location), ToolName);
        }

        protected override string ToolName                 => "nav.exe";
        protected override Encoding ResponseFileEncoding   => Encoding.UTF8;
        protected override Encoding StandardOutputEncoding => Encoding.UTF8;

        protected override string GenerateResponseFileCommands() {

            var clb = new CommandLineBuilder();

            clb.AppendSwitchIfPresent(GenerateToClasses, "/t");
            clb.AppendSwitchIfPresent(Force,             "/f");
            clb.AppendSwitchIfPresent(UseSyntaxCache,    "/c");
            clb.AppendSwitchIfPresent(FullPaths,         "/fullpaths");
            clb.AppendSwitch("/v");
            clb.AppendSwitchIfNotNull("/r:", ProjectRootDirectory);
            clb.AppendSwitchIfNotNull("/i:", IwflRootDirectory);
            clb.AppendSwitchIfNotNull("/s:", Sources, " /s:");

            return clb.ToString();
        }

        protected override bool ValidateParameters() {            
            return true;
        }
        
        protected override bool SkipTaskExecution() {
            return (Sources?.Length ?? 0) == 0;
        }
        
        protected override void LogEventsFromTextOutput(string singleLine, MessageImportance messageImportance) {

            const string verbosePrefix = "Verbose:";

            if (singleLine.StartsWith(verbosePrefix)) {
                messageImportance = MessageImportance.Low;
                singleLine = singleLine.Substring(verbosePrefix.Length);
            }

            base.LogEventsFromTextOutput(singleLine, messageImportance);            
        }

        protected override MessageImportance StandardOutputLoggingImportance => MessageImportance.High;
    }
}