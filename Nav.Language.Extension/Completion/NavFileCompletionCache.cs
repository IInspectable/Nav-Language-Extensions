#region Using Directives

using System;
using System.Collections.Immutable;
using System.ComponentModel.Composition;
using System.IO;
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

    // TODO Naming...
    [Export]
    class NavFileCompletionCache {

        DirectoryInfo _directory;

        private DateTime _lastChanged = DateTime.Now;

        private FileInfoSnapshot _fileInfoSnapshot;

        readonly FileSystemWatcher _fileSystemWatcher;

        [ImportingConstructor]
        public NavFileCompletionCache() {

            _fileInfoSnapshot = FileInfoSnapshot.Empty;

            SolutionEvents.OnAfterCloseSolution                  += OnAfterCloseSolution;
            SolutionEvents.OnAfterOpenSolution                   += OnAfterOpenSolution;
            SolutionEvents.OnAfterBackgroundSolutionLoadComplete += OnAfterBackgroundSolutionLoadComplete;

            _fileSystemWatcher = new FileSystemWatcher {
                Filter                = "*.nav",
                IncludeSubdirectories = true,
                NotifyFilter          = NotifyFilters.CreationTime | NotifyFilters.FileName | NotifyFilters.DirectoryName
            };

            _fileSystemWatcher.Renamed += OnFileSystemRenamed;
            _fileSystemWatcher.Created += OnFileSystemCreated;
            _fileSystemWatcher.Deleted += OnFileSystemDeleted;

            UpdateSearchDirectory();

        }

        private void OnFileSystemDeleted(object sender, FileSystemEventArgs e) {
            Invalidate();
        }

        private void OnFileSystemCreated(object sender, FileSystemEventArgs e) {
            Invalidate();
        }

        private void OnFileSystemRenamed(object sender, RenamedEventArgs e) {
            Invalidate();
        }

        void Invalidate() {
            _lastChanged      = DateTime.Now;
            _fileInfoSnapshot = FileInfoSnapshot.Empty;
        }

        public ImmutableArray<FileInfo> GetNavFiles(CancellationToken cancellationToken) {

            if (_directory == null) {
                return ImmutableArray<FileInfo>.Empty;
            }

            if (_fileInfoSnapshot.IsCurrent(_directory, _lastChanged)) {
                return _fileInfoSnapshot.FileInfos;
            }

            _fileInfoSnapshot = CreateFileInfoSnapshot(_directory, cancellationToken);

            return _fileInfoSnapshot.FileInfos;
        }

        static FileInfoSnapshot CreateFileInfoSnapshot(DirectoryInfo directory, CancellationToken cancellationToken) {

            var creationTime = DateTime.Now;
            var itemBuilder  = ImmutableArray.CreateBuilder<FileInfo>();

            foreach (var file in Directory.EnumerateFiles(
                directory.FullName,
                $"*{NavLanguageContentDefinitions.FileExtension}",
                SearchOption.AllDirectories)) {

                if (cancellationToken.IsCancellationRequested) {
                    return FileInfoSnapshot.Empty;
                }

                var fileInfo = new FileInfo(file);
                itemBuilder.Add(fileInfo);

            }

            return new FileInfoSnapshot(directory, creationTime, itemBuilder.ToImmutableArray());
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

        void OnAfterOpenSolution(object sender, OpenSolutionEventArgs e) {
            UpdateSearchDirectory();
        }

        void OnAfterCloseSolution(object sender, EventArgs e) {
            UpdateSearchDirectory();
        }

        void OnAfterBackgroundSolutionLoadComplete(object sender, EventArgs e) {
            UpdateSearchDirectory();
        }

    }

}