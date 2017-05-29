#region Using Directives

using System;
using Fclp;

#endregion

namespace Pharmatechnik.Nav.Language {

    sealed class CommandLine {

        public string Directory { get; set; }
        public bool Force { get; set; }
        public bool GenerateToClasses { get; set; }
        public bool UseSyntaxCache { get; set; }
        public bool Verbose { get; set; }
        public bool Analyze { get; set; }
        public string Pattern { get; set; }

        public static CommandLine Parse(string[] commandline) {

            var clp = new FluentCommandLineParser<CommandLine>();
            clp.Setup(i => i.Directory).As('d', nameof(Directory)).Required().WithDescription("Directory to search for nav files");
            clp.Setup(i => i.Force).As('f', nameof(Force)).SetDefault(false);
            clp.Setup(i => i.GenerateToClasses).As('g', nameof(GenerateToClasses)).SetDefault(true);
            clp.Setup(i => i.UseSyntaxCache).As('c', nameof(UseSyntaxCache)).SetDefault(false);
            clp.Setup(i => i.Verbose).As('v', nameof(Verbose)).SetDefault(false);
            clp.Setup(i => i.Analyze).As('a', nameof(Analyze)).SetDefault(false);
            clp.Setup(i => i.Pattern).As('p', nameof(Pattern)).SetDefault("*");

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