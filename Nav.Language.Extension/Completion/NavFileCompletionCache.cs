#region Using Directives

using System.Collections.Immutable;
using System.ComponentModel.Composition;
using System.IO;
using System.Threading;

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Events;
using Microsoft.VisualStudio.Shell.Interop;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Completion {

    // TODO Naming...
    [Export]
    class NavFileCompletionCache {

        readonly IVsSolution _solution;
        string               _directory;

        [ImportingConstructor]
        public NavFileCompletionCache() {

            _solution = NavLanguagePackage.GetGlobalService<SVsSolution, IVsSolution>();

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

            _directory = GetSolutionDirectory();
        }

        bool IsSolutionOpen {
            get {
                ThreadHelper.ThrowIfNotOnUIThread();

                _solution.GetProperty((int) __VSPROPID.VSPROPID_IsSolutionOpen, out object value);

                return value is bool isSolOpen && isSolOpen;
            }
        }

        string GetSolutionDirectory() {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (!IsSolutionOpen) {
                return null;
            }

            if (ErrorHandler.Succeeded(_solution.GetSolutionInfo(out var solutionDirectory, out _, out _))) {
                return solutionDirectory;
            }

            return null;
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