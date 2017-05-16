#region Using Directives

using System;

using Fclp;

#endregion

namespace TestClient {

    sealed class CommandLine {

        public string Directory { get; set; }
        public bool Force { get; set; }

        public static CommandLine Parse(string[] commandline) {

            var clp = new FluentCommandLineParser<CommandLine>();
            clp.Setup(i => i.Directory).As('d', nameof(Directory)).Required();
            clp.Setup(i => i.Force).As('f', nameof(Force)).SetDefault(false);

            var result = clp.Parse(commandline);
            if (result.HasErrors) {
                Console.Error.WriteLine($"Unable to parse command line:\n{result.ErrorText}");
                return null;
            }

            CommandLine cla = clp.Object;

            return cla;
        }
    }
}