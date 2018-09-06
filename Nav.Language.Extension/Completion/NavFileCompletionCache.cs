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

namespace Pharmatechnik.Nav.Language.Extension.Completion {

    [Export]
    class NavFileCompletionCache {

        ImmutableList<FileInfo>            _fileCache;
        private readonly IVsSolution       _solution;
        readonly         FileSystemWatcher _fileSystemWatcher;

        Task                    _cacheTask;
        CancellationTokenSource _cts;

        [ImportingConstructor]
        public NavFileCompletionCache() {

            _solution  = NavLanguagePackage.GetGlobalService<SVsSolution, IVsSolution>();
            _fileCache = ImmutableList<FileInfo>.Empty;
            _cts       = new CancellationTokenSource();
            
            _fileSystemWatcher = new FileSystemWatcher {
                IncludeSubdirectories = true,
                Filter                = "*.{NavLanguageContentDefinitions.FileExtension}",
                NotifyFilter          = NotifyFilters.FileName | NotifyFilters.DirectoryName
            };

            _fileSystemWatcher.Renamed += (o, e) => OnFileSystemChanged();
            _fileSystemWatcher.Deleted += (o, e) => OnFileSystemChanged();

            RefreshCache();

            SolutionEvents.OnAfterCloseSolution                  += OnAfterCloseSolution;
            SolutionEvents.OnAfterOpenSolution                   += OnAfterOpenSolution;
            SolutionEvents.OnAfterBackgroundSolutionLoadComplete += OnAfterBackgroundSolutionLoadComplete;

        }

        void OnFileSystemChanged() {
            #pragma warning disable 219
            int i = 0;
            #pragma warning restore 219
        }

        public bool IsBuilding() {
            return !_cacheTask.IsCompleted;
        }

        public ImmutableList<FileInfo> GetNavFiles() {
            return _fileCache;
        }

        void ClearCache() {
            _fileCache                             = ImmutableList<FileInfo>.Empty;
            _fileSystemWatcher.EnableRaisingEvents = false;
        }

        void RefreshCache() {
            _cts.Cancel();
            _cts = new CancellationTokenSource();
            ClearCache();
            _cacheTask = RefreshCacheAsync(_cts.Token);
        }

        async Task RefreshCacheAsync(CancellationToken cancellationToken) {

            string directory = GetSolutionDirectory();

            if (string.IsNullOrEmpty(directory)) {
                return;
            }

            if (_fileSystemWatcher.Path != directory) {
                _fileSystemWatcher.Path                = directory;
                _fileSystemWatcher.EnableRaisingEvents = true;
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