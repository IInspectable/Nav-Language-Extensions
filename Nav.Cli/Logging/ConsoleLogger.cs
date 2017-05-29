#region Using Directives

using System;
using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language.Logging {

    public sealed class ConsoleLogger : ILogger {

        public ConsoleLogger(bool verbose) {
            Verbose = verbose;
        }

        public bool Verbose { get; }

        public void LogVerbose(string message) {
            if (!Verbose) {
                return;
            }
            WriteVerbose(message);
        }
        
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

        public const string VerbosePrefix = "Verbose:";
        void WriteVerbose(string message) {
            WriteLine($"{VerbosePrefix}{message}", ConsoleColor.DarkGray);
        }

        void WriteInfo(string message) {
            WriteLine($"{message}", Console.ForegroundColor);
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
            return $"{LogHelper.GetFileIdentity(diag, fileSpec)}({location.StartLine + 1},{location.StartCharacter + 1}): {GetSeverity(diag)} {diag.Descriptor.Id}: {diag.Message}";
        }

        string GetSeverity(Diagnostic diag) {
            return diag.Severity.ToString();
        }
    }
}