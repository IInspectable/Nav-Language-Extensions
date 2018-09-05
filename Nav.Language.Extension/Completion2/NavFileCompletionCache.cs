#region Using Directives

using System.Collections.Immutable;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading;

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Events;
using Microsoft.VisualStudio.Shell.Interop;

using Task = System.Threading.Tasks.Task;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Completion2 {

    [Export]
    class NavFileCompletionCache {

        ImmutableList<FileInfo>      _fileCache;
        private readonly IVsSolution _solution;

        Task                    _cacheTask;
        CancellationTokenSource _cts;

        [ImportingConstructor]
        public NavFileCompletionCache() {

            _solution  = NavLanguagePackage.GetGlobalService<SVsSolution, IVsSolution>();
            _fileCache = ImmutableList<FileInfo>.Empty;
            _cts       = new CancellationTokenSource();

            SolutionEvents.OnAfterCloseSolution                  += OnAfterCloseSolution;
            SolutionEvents.OnAfterOpenSolution                   += OnAfterOpenSolution;
            SolutionEvents.OnAfterBackgroundSolutionLoadComplete += OnAfterBackgroundSolutionLoadComplete;

            RefreshCache();

            // TODO FileSystemWatcher
            //FileSystemWatcher fileSystemWatcher = new FileSystemWatcher();
            //fileSystemWatcher.
        }

        public bool IsBuilding() {
            return !_cacheTask.IsCompleted;
        }

        public ImmutableList<FileInfo> GetNavFiles() {
            return _fileCache;
        }

        void ClearCache() {
            _fileCache = ImmutableList<FileInfo>.Empty;
        }

        void RefreshCache() {
            _cts.Cancel();
            _cts = new CancellationTokenSource();
            ClearCache();
            _cacheTask = RefreshCacheAsync(_cts.Token);
        }

        async Task RefreshCacheAsync(CancellationToken cancellationToken) {

            //   await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            string directory = GetSolutionDirectory();

            if (string.IsNullOrEmpty(directory)) {
                return;
            }

            await Task.Run(() => {
                    _fileCache = Directory.EnumerateFiles(
                                               directory,
                                               $"*{NavLanguageContentDefinitions.FileExtension}",
                                               SearchOption.AllDirectories)
                                          .Select(f => new FileInfo(f))
                                          .ToImmutableList();
                }, cancellationToken
            );
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
                return string.Empty;
            }

            if (ErrorHandler.Succeeded(_solution.GetSolutionInfo(out var solutionDirectory, out _, out _))) {
                return solutionDirectory;
            }

            return string.Empty;
        }

        void OnAfterOpenSolution(object sender, OpenSolutionEventArgs e) {
            RefreshCache();
        }

        void OnAfterCloseSolution(object sender, System.EventArgs e) {
            ClearCache();
        }

        void OnAfterBackgroundSolutionLoadComplete(object sender, System.EventArgs e) {
            RefreshCache();
        }

    }

}