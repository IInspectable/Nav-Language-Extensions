#region Using Directives

using System;
using System.Collections.Immutable;
using System.ComponentModel.Composition;
using System.IO;
using System.Reactive.Linq;
using System.Threading;

using Microsoft.VisualStudio.Shell.Events;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Completion {

    class FileInfoSnapshot {

        FileInfoSnapshot() {
            CreationTime = DateTime.MinValue;
            FileInfos    = ImmutableArray<FileInfo>.Empty;
            Directory    = null;
        }

        public FileInfoSnapshot(DirectoryInfo directoryInfo, DateTime creationTime, ImmutableArray<FileInfo> fileInfos) {
            CreationTime = creationTime;
            Directory    = directoryInfo ?? throw new ArgumentNullException(nameof(directoryInfo));
            FileInfos    = fileInfos;
        }

        public static readonly FileInfoSnapshot Empty = new FileInfoSnapshot();

        public DirectoryInfo            Directory    { get; }
        public DateTime                 CreationTime { get; }
        public ImmutableArray<FileInfo> FileInfos    { get; }

        public bool IsCurrent(DirectoryInfo directory, DateTime lastFileSystemChange) {

            if (directory == null || Directory == null) {
                return false;
            }

            return directory.FullName   == Directory.FullName &&
                   lastFileSystemChange <= CreationTime;
        }

    }

    //  TODO Sollt wohl in Zukunft irgendwie mehr so etwas wie ein VsNavWorkspace werden!?
    [Export]
    class NavFileProvider {

        DirectoryInfo _directory;

        private DateTime _lastChanged = DateTime.Now;

        private FileInfoSnapshot _fileInfoSnapshot;

        readonly FileSystemWatcher _fileSystemWatcher;

        static string SearchFilter => $"*{NavLanguageContentDefinitions.FileExtension}";

        [ImportingConstructor]
        public NavFileProvider() {

            _fileInfoSnapshot = FileInfoSnapshot.Empty;

            SolutionEvents.OnAfterCloseSolution                  += OnAfterCloseSolution;
            SolutionEvents.OnAfterOpenSolution                   += OnAfterOpenSolution;
            SolutionEvents.OnAfterBackgroundSolutionLoadComplete += OnAfterBackgroundSolutionLoadComplete;

            _fileSystemWatcher = new FileSystemWatcher {
                Filter                = SearchFilter,
                IncludeSubdirectories = true,
                NotifyFilter          = NotifyFilters.CreationTime | NotifyFilters.FileName | NotifyFilters.DirectoryName
            };

            _fileSystemWatcher.Renamed += OnFileSystemRenamed;
            _fileSystemWatcher.Created += OnFileSystemCreated;
            _fileSystemWatcher.Deleted += OnFileSystemDeleted;

            UpdateSearchDirectory();

            // TODO Dispose beim Beenden von Studio
            Observable.FromEventPattern<EventArgs>(handler => Invalidated += handler,
                                                   handler => Invalidated -= handler)
                      .Throttle(TimeSpan.FromSeconds(2))
                      .Select(_ => CreateFileInfoSnapshot(_directory, CancellationToken.None))
                      .Subscribe(TrySetFileInfoSnapshot);
        }

        private event EventHandler<EventArgs> Invalidated;

        void OnAfterOpenSolution(object sender, OpenSolutionEventArgs e) {
            UpdateSearchDirectory();
        }

        void OnAfterCloseSolution(object sender, EventArgs e) {
            UpdateSearchDirectory();
        }

        void OnAfterBackgroundSolutionLoadComplete(object sender, EventArgs e) {
            UpdateSearchDirectory();
        }

        void UpdateSearchDirectory() {

            _directory = NavLanguagePackage.SearchDirectory;

            if (_directory == null) {
                _fileSystemWatcher.EnableRaisingEvents = false;
            } else {
                _fileSystemWatcher.Path                = _directory.FullName;
                _fileSystemWatcher.EnableRaisingEvents = true;
            }

            Invalidate();
        }

        void OnFileSystemDeleted(object sender, FileSystemEventArgs e) {
            Invalidate();
        }

        void OnFileSystemCreated(object sender, FileSystemEventArgs e) {
            Invalidate();
        }

        void OnFileSystemRenamed(object sender, RenamedEventArgs e) {
            Invalidate();
        }

        void Invalidate() {

            _lastChanged = DateTime.Now;

            OnInvalidated();
        }

        void OnInvalidated() {
            Invalidated?.Invoke(this, EventArgs.Empty);
        }

        public ImmutableArray<FileInfo> GetNavFiles(CancellationToken cancellationToken) {

            if (_fileInfoSnapshot.IsCurrent(_directory, _lastChanged)) {
                return _fileInfoSnapshot.FileInfos;
            }

            var fileInfoSnapshot = CreateFileInfoSnapshot(_directory, cancellationToken);

            TrySetFileInfoSnapshot(fileInfoSnapshot);

            return _fileInfoSnapshot.FileInfos;
        }

        static FileInfoSnapshot CreateFileInfoSnapshot(DirectoryInfo directory, CancellationToken cancellationToken) {

            if (String.IsNullOrEmpty(directory?.FullName)) {
                return FileInfoSnapshot.Empty;
            }

            var creationTime = DateTime.Now;
            var itemBuilder  = ImmutableArray.CreateBuilder<FileInfo>();

            foreach (var file in Directory.EnumerateFiles(
                directory.FullName,
                SearchFilter,
                SearchOption.AllDirectories)) {

                if (cancellationToken.IsCancellationRequested) {
                    return FileInfoSnapshot.Empty;
                }

                var fileInfo = new FileInfo(file);
                itemBuilder.Add(fileInfo);

            }

            return new FileInfoSnapshot(directory, creationTime, itemBuilder.ToImmutableArray());
        }

        private readonly object _gate = new object();

        void TrySetFileInfoSnapshot(FileInfoSnapshot fileInfoSnapshot) {

            lock (_gate) {

                if (!fileInfoSnapshot.IsCurrent(_directory, _lastChanged)) {
                    return;
                }

                _fileInfoSnapshot = fileInfoSnapshot;
            }

        }

    }

}