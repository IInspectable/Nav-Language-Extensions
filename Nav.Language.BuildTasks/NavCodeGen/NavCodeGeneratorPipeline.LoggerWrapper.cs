#region Using Directives

using System.Collections.Generic;

using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language.BuildTasks {

    public sealed partial class NavCodeGeneratorPipeline {
        public sealed class LoggerWrapper {

            public LoggerWrapper(IGeneratorLogger logger) {
                Logger = logger;
            }

            [CanBeNull]
            IGeneratorLogger Logger { get; }

            public bool HasLoggedErrors { get; set; }

            public void LogError(string message) {
                HasLoggedErrors = true;
                Logger?.LogError(message);
            }

            public bool LogErrors(FileSpec fileSpec, IEnumerable<Diagnostic> diagnostics) {

                bool errorsLogged = false;
                foreach (var error in diagnostics.Errors()) {
                    errorsLogged = true;
                    HasLoggedErrors = true;
                    Logger?.LogError(error, fileSpec);
                }
                return errorsLogged;
            }

            public void LogWarnings(FileSpec fileSpec, IEnumerable<Diagnostic> diagnostics) {
                foreach (var warning in diagnostics.Warnings()) {
                    Logger?.LogWarning(warning, fileSpec);
                }
            }
        }
    }
}