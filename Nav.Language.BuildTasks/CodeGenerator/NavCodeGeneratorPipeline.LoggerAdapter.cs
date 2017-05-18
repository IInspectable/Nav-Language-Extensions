#region Using Directives

using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.Immutable;

using JetBrains.Annotations;

using Pharmatechnik.Nav.Utilities.IO;
using Pharmatechnik.Nav.Language.CodeGen;

#endregion

namespace Pharmatechnik.Nav.Language.BuildTasks {

    public sealed partial class NavCodeGeneratorPipeline {
        sealed class LoggerAdapter {

            [CanBeNull] readonly ILogger _logger;

            public LoggerAdapter(ILogger logger) {
                _logger              = logger;
                ProcessStopwatch     = new Stopwatch();
                ProcessFileStopwatch = new Stopwatch();
            }
            
            [NotNull]
            Stopwatch ProcessStopwatch { get; }
            [NotNull]
            Stopwatch ProcessFileStopwatch { get; }

            public bool HasLoggedErrors { get; private set; }
           
            public void LogError(string message) {
                HasLoggedErrors = true;
                _logger?.LogError(message);
            }

            public bool LogErrors(FileSpec fileSpec, IEnumerable<Diagnostic> diagnostics) {

                bool errorsLogged = false;
                foreach (var error in diagnostics.Errors()) {
                    errorsLogged = true;
                    HasLoggedErrors = true;
                    _logger?.LogError(error, fileSpec);
                }
                return errorsLogged;
            }

            public void LogWarnings(FileSpec fileSpec, IEnumerable<Diagnostic> diagnostics) {
                foreach (var warning in diagnostics.Warnings()) {
                    _logger?.LogWarning(warning, fileSpec);
                }
            }

            public void LogProcessBegin() {
                _logger?.LogInfo($"{ThisAssembly.ProductName} v{ThisAssembly.ProductVersion}");
                ProcessStopwatch.Restart();
            }

            public void LogProcessFileBegin(FileSpec fileSpec) {
                ProcessFileStopwatch.Restart();
                _logger?.LogVerbose($"Processing file '{fileSpec.Identity}'");
            }
            
            public void LogFileGeneratorResults(IImmutableList<FileGeneratorResult> fileResults) {

                var longestName = fileResults.Select(r => r.Action.ToString().Length).Max();

                foreach (var fileResult in fileResults) {

                    var fileIdentity = fileResult.FileName;
                    
                    var syntaxDirectory = fileResult.TaskDefinition.Syntax.SyntaxTree.FileInfo?.DirectoryName;
                    if (syntaxDirectory != null) {
                        fileIdentity = PathHelper.GetRelativePath(syntaxDirectory, fileResult.FileName);
                    }

                    var action  = fileResult.Action.ToString().PadRight(longestName);
                    var message = $"   {action}: {fileIdentity}";
                    _logger?.LogVerbose(message);
                }
            }

            // ReSharper disable once UnusedParameter.Local
            public void LogProcessFileEnd(FileSpec fileSpec) {
                ProcessFileStopwatch.Stop();
                _logger?.LogVerbose($"Completed in {ProcessFileStopwatch.Elapsed.TotalSeconds} seconds.");
            }

            public void LogProcessEnd(Statistic statistic) {
                ProcessStopwatch.Stop();

                _logger?.LogInfo($"{statistic.FileCount} {Pluralize("file", statistic.FileCount)} with {statistic.TaskCount} {Pluralize("task", statistic.TaskCount)} processed.");
                _logger?.LogInfo($"   Updated: {statistic.FilesUpated,3} {Pluralize("File", statistic.FilesUpated)}");
                _logger?.LogInfo($"   Skiped : {statistic.FilesSkiped,3} {Pluralize("File", statistic.FilesSkiped)}");
                _logger?.LogInfo($"Completed in {ProcessStopwatch.Elapsed.TotalSeconds} seconds.");
            }

            string Pluralize(string word, int count) {
                if (count == 1) {
                    return word;
                } 
                return $"{word}s";
            }
        }
    }
}