#region Using Directives

using System;

#endregion

namespace Pharmatechnik.Nav.Language.BuildTasks {
    public class ConsoleGeneratorLogger : IGeneratorLogger {

        public void LogError(string message) {
            WriteError(message);
        }

        public void LogError(Diagnostic diag) {
            WriteError(FormatDiagnostic(diag));
        }

        public void LogWarning(Diagnostic diag) {
            WriteWarning(FormatDiagnostic(diag));
        }


        private void WriteError(string message) {
            WriteLine(message, ConsoleColor.Red);
        }

        private void WriteWarning(string message) {
            WriteLine(message, ConsoleColor.Yellow);
        }

        private void WriteLine(string message, ConsoleColor backgroundColor) {
            var oldBackground = Console.BackgroundColor;
            try {
                Console.BackgroundColor = backgroundColor;
                Console.WriteLine(message);
            } finally {
                Console.BackgroundColor = oldBackground;
            }
        }

        string FormatDiagnostic(Diagnostic diag) {
            // TODO Line Info etc...
            return diag.Message;
        }
    }
}