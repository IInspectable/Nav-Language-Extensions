#region Using Directives

using System;
using System.Collections.Generic;
using Fclp;

#endregion

namespace Pharmatechnik.Nav.Language {

    sealed class CommandLine {
        public CommandLine() {
            Sources = new List<string>();
        }

        public string Directory { get; set; }
        public List<string> Sources { get; private set; }
        public bool Force { get; set; }
        public bool GenerateToClasses { get; set; }
        public bool UseSyntaxCache { get; set; }
        public bool Verbose { get; set; }
        public bool FullPaths { get; set; }

        public bool Analyze { get; set; }
        public string Pattern { get; set; }

        public static CommandLine Parse(string[] commandline) {

            var clp = new FluentCommandLineParser<CommandLine>();

            clp.Setup(i => i.Directory)        .As('d', "directory")        .WithDescription("Directory to search for nav files");
            clp.Setup(i => i.Sources)          .As('s', "sources");
            clp.Setup(i => i.Force)            .As('f', "force")            .SetDefault(false);
            clp.Setup(i => i.GenerateToClasses).As('t', "generatetoclasses").SetDefault(true);
            clp.Setup(i => i.UseSyntaxCache)   .As('c', "useSyntaxCache")   .SetDefault(false);
            clp.Setup(i => i.Verbose)          .As('v', "verbose")          .SetDefault(false);
            clp.Setup(i => i.FullPaths)        .As(     "fullpaths")        .SetDefault(false);
            // Analyze parameter
            clp.Setup(i => i.Analyze)          .As('a', nameof(Analyze)).SetDefault(false);
            clp.Setup(i => i.Pattern)          .As('p', nameof(Pattern)).SetDefault("*");

            clp.SetupHelp("?", "help").Callback(text => Console.WriteLine(text)); 

            var result = clp.Parse(commandline);
            if (result.HasErrors) {
                Console.Error.WriteLine($"Unable to parse command line:\n{result.ErrorText}");
                return null;
            }

            if (result.HelpCalled) {
                return null;
            }

            CommandLine cla = clp.Object;

            return cla;
        }
    }
}