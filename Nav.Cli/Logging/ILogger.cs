#region Using Directives

using JetBrains.Annotations;
using Pharmatechnik.Nav.Language.Generator;

#endregion

namespace Pharmatechnik.Nav.Language.BuildTasks {
    public interface ILogger {
        void LogVerbose(string message);
        void LogInfo(string message);
        void LogWarning(Diagnostic diag, [CanBeNull] FileSpec fileSpec = null);
        void LogError(string message);
        void LogError(Diagnostic diag  , [CanBeNull] FileSpec fileSpec = null);
    }
}