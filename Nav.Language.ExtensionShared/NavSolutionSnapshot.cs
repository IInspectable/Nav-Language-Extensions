#region Using Directives

using System;
using System.IO;

using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language.Extension;

readonly struct NavSolutionSnapshot {

    public NavSolutionSnapshot(DateTime creationTime, NavSolution solution) {
        CreationTime = creationTime;
        Solution     = solution;

    }

    public static readonly NavSolutionSnapshot Empty = new(DateTime.MinValue, NavSolution.Empty);

    public DateTime CreationTime { get; }

    [NotNull]
    public NavSolution Solution { get; }

    public bool IsCurrent(DirectoryInfo solutionDirectory, DateTime lastFileSystemChange) {

        if (solutionDirectory == null || Solution.SolutionDirectory == null) {
            return false;
        }

        return solutionDirectory.FullName == Solution.SolutionDirectory.FullName &&
               lastFileSystemChange       <= CreationTime;
    }

}