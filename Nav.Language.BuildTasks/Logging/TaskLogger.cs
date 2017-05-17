#region Using Directives

using System;

using JetBrains.Annotations;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

#endregion

namespace Pharmatechnik.Nav.Language.BuildTasks {

    public class TaskLogger : ILogger {

        public TaskLogger(Task task) {
            Log = task.Log ?? throw new ArgumentNullException(nameof(task));
        }

        TaskLoggingHelper Log { get; }

        public void LogVerbose(string message) {
            Log.LogMessage(MessageImportance.Low, message);
        }

        public void LogInfo(string message) {
            Log.LogMessage(MessageImportance.Normal, message);
        }

        public void LogError(string message) {
            Log.LogError(message);
        }

        public void LogError(Diagnostic diag, [CanBeNull] FileSpec fileSpec) {
            Log.LogError(
                subcategory    : null,
                errorCode      : diag.Descriptor.Id,
                helpKeyword    : null,
                file           : LogHelper.GetFileIdentity(diag, fileSpec),
                lineNumber     : diag.Location.StartLine + 1,
                columnNumber   : diag.Location.StartCharacter + 1,
                endLineNumber  : 0, // Ist das von Interesse?
                endColumnNumber: 0, // Ist das von Interesse?
                message        : diag.Message);
        }

        public void LogWarning(Diagnostic diag, [CanBeNull] FileSpec fileSpec) {
            Log.LogWarning(
                subcategory    : null,
                warningCode    : diag.Descriptor.Id,
                helpKeyword    : null,
                file           : LogHelper.GetFileIdentity(diag, fileSpec),
                lineNumber     : diag.Location.StartLine + 1,
                columnNumber   : diag.Location.StartCharacter + 1,
                endLineNumber  : 0, // Ist das von Interesse?
                endColumnNumber: 0, // Ist das von Interesse?
                message        : diag.Message);
        }      
    }
}