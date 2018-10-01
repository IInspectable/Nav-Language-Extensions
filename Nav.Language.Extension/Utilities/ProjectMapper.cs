#region Using Directives

using System.Linq;
using System.Collections.Immutable;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Utilities {

    class ProjectMapper {

        private readonly ImmutableArray<ProjectEntry> _projectEntries;

        public ProjectMapper(ImmutableArray<ProjectEntry> projectEntries) {
            _projectEntries = projectEntries;

        }

        public static string MiscellaneousFiles = "Miscellaneous Files";

        public static readonly ProjectMapper Empty = new ProjectMapper(ImmutableArray<ProjectEntry>.Empty);

        public string GetContainingProjectName(string fileName) {

            var uri = UriBuilder.BuildDirectoryUriFromFile(fileName);
            if (uri == null) {
                return MiscellaneousFiles;
            }

            return _projectEntries.FirstOrDefault(pe => pe.ProjectDirectory.IsBaseOf(uri)).Name ?? MiscellaneousFiles;
        }

    }

}