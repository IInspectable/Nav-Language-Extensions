#region Using Directives

using System;
using System.Collections.Immutable;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language {

    public class NavSolution {

        public NavSolution([CanBeNull] DirectoryInfo solutionRoot, ImmutableArray<FileInfo> solutionFiles) {
            SolutionDirectory = solutionRoot;
            SolutionFiles     = solutionFiles;
        }

        [CanBeNull]
        public DirectoryInfo SolutionDirectory { get; }

        public ImmutableArray<FileInfo> SolutionFiles { get; }

        public static NavSolution Empty = new NavSolution(null, ImmutableArray<FileInfo>.Empty);

        public static string SearchFilter => $"*.nav";

        public static Task<NavSolution> FromDirectoryAsync(DirectoryInfo directory, CancellationToken cancellationToken) {

            if (String.IsNullOrEmpty(directory?.FullName)) {
                return Task.FromResult(Empty);
            }

            var itemBuilder = ImmutableArray.CreateBuilder<FileInfo>();

            foreach (var file in Directory.EnumerateFiles(directory.FullName,
                                                          SearchFilter,
                                                          SearchOption.AllDirectories)) {

                if (cancellationToken.IsCancellationRequested) {
                    return Task.FromResult(Empty);
                }

                var fileInfo = new FileInfo(file);
                itemBuilder.Add(fileInfo);

            }

            var solution = new NavSolution(directory, itemBuilder.ToImmutableArray());

            return Task.FromResult(solution);
        }

    }

}