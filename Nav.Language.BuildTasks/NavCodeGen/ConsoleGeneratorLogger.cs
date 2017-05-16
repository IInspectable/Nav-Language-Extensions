#region Using Directives

using System;
using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language.BuildTasks {
    public sealed class ConsoleGeneratorLogger : IGeneratorLogger {

        public void LogInfo(string message) {
            WriteInfo(message);
        }

        public void LogError(string message) {
            WriteError(message);
        }

        public void LogError(Diagnostic diag, [CanBeNull] FileSpec fileSpec = null) {
            WriteError(FormatDiagnostic(diag, fileSpec));
        }

        public void LogWarning(Diagnostic diag, [CanBeNull] FileSpec fileSpec = null) {
            WriteWarning(FormatDiagnostic(diag, fileSpec));
        }

        void WriteInfo(string message) {
            WriteLine(message, ConsoleColor.DarkGray);
        }

        void WriteError(string message) {
            WriteLine(message, ConsoleColor.Red);
        }

        void WriteWarning(string message) {
            WriteLine(message, ConsoleColor.Yellow);
        }

        void WriteLine(string message, ConsoleColor backgroundColor) {
            var oldBackground = Console.ForegroundColor;
            try {
                Console.ForegroundColor = backgroundColor;
                Console.WriteLine(message);
            } finally {
                Console.ForegroundColor = oldBackground;
            }
        }

        string FormatDiagnostic(Diagnostic diag, FileSpec fileSpec) {
            var location = diag.Location;
            return $"{GetFile(diag, fileSpec)}({location.StartLine+1},{location.StartCharacter+1}): {GetSeverity(diag)} {diag.Descriptor.Id}: {diag.Message}";
        }

        string GetSeverity(Diagnostic diag) {
            switch (diag.Severity) {
                case DiagnosticSeverity.Suggestion:
                    return "Suggestion";
                case DiagnosticSeverity.Warning:
                    return "Warning";
                case DiagnosticSeverity.Error:
                    return "Error";
                default:
                    return String.Empty;
            }
        }

        static string GetFile(Diagnostic diag, [CanBeNull] FileSpec fileSpec) {
            if (diag?.Location.FilePath?.ToLower() == fileSpec?.FilePath.ToLower()) {
                return fileSpec?.Identity ?? diag?.Location.FilePath;
            }
            return diag?.Location.FilePath;
        }
    }
}