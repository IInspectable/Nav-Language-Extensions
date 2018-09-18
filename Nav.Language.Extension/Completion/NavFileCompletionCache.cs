#region Using Directives

using System.Collections.Immutable;
using System.ComponentModel.Composition;
using System.IO;
using System.Threading;

using Microsoft.VisualStudio.Shell.Events;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Completion {

    // TODO Naming...
    [Export]
    class NavFileCompletionCache {

        string _directory;

        [ImportingConstructor]
        public NavFileCompletionCache() {

            UpdateSearchDirectory();

            SolutionEvents.OnAfterCloseSolution                  += OnAfterCloseSolution;
            SolutionEvents.OnAfterOpenSolution                   += OnAfterOpenSolution;
            SolutionEvents.OnAfterBackgroundSolutionLoadComplete += OnAfterBackgroundSolutionLoadComplete;

        }

        public ImmutableArray<FileInfo> GetNavFiles(CancellationToken cancellationToken) {

            if (string.IsNullOrEmpty(_directory)) {
                return ImmutableArray<FileInfo>.Empty;
            }

            var itemBuilder = ImmutableArray.CreateBuilder<FileInfo>();

            foreach (var file in Directory.EnumerateFiles(
                _directory,
                $"*{NavLanguageContentDefinitions.FileExtension}",
                SearchOption.AllDirectories)) {

                if (cancellationToken.IsCancellationRequested) {
                    return ImmutableArray<FileInfo>.Empty;
                }

                var fileInfo = new FileInfo(file);
                itemBuilder.Add(fileInfo);

            }

            return itemBuilder.ToImmutableArray();
        }

        void UpdateSearchDirectory() {
            _directory = NavLanguagePackage.SearchDirectory?.FullName;
        }

        void OnAfterOpenSolution(object sender, OpenSolutionEventArgs e) {
            UpdateSearchDirectory();
        }

        void OnAfterCloseSolution(object sender, System.EventArgs e) {
            UpdateSearchDirectory();
        }

        void OnAfterBackgroundSolutionLoadComplete(object sender, System.EventArgs e) {
            UpdateSearchDirectory();
        }

    }

}