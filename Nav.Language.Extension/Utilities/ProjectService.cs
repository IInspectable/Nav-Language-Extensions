#region Using Directives

using System;
using System.Collections.Immutable;
using System.ComponentModel.Composition;

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Utilities {

    [Export(typeof(ProjectService))]
    class ProjectService {

        readonly IVsSolution _vsSolution1;

        [ImportingConstructor]
        public ProjectService(SVsServiceProvider serviceProvider) {

            ThreadHelper.ThrowIfNotOnUIThread();

            _vsSolution1 = (IVsSolution) serviceProvider.GetService(typeof(SVsSolution)) ?? throw new InvalidOperationException();
        }

        public ProjectMapper GetProjectMapper() {

            ThreadHelper.ThrowIfNotOnUIThread();

            var entries = ImmutableArray.CreateBuilder<ProjectInfo>();

            Guid ignored = Guid.Empty;
            var  flags   = __VSENUMPROJFLAGS.EPF_LOADEDINSOLUTION | __VSENUMPROJFLAGS.EPF_UNLOADEDINSOLUTION;
            if (ErrorHandler.Failed(_vsSolution1.GetProjectEnum((uint) flags, ref ignored, out var hierEnum))) {
                return ProjectMapper.Empty;
            }

            IVsHierarchy[] hier = new IVsHierarchy[1];
            while (hierEnum.Next((uint) hier.Length, hier, out var fetched) == VSConstants.S_OK && fetched == hier.Length) {

                var hierarchy = new Hierarchy(hier[0]);

                var directory   = UriBuilder.BuildDirectoryUriFromFile(hierarchy.FullPath);
                var name        = hierarchy.Name;
                var projectGuid = hierarchy.ProjectGuid;

                if (directory == null || name == null) {
                    continue;
                }

                entries.Add(new ProjectInfo(directory, name, projectGuid));

            }

            return new ProjectMapper(entries.ToImmutable());
        }

    }

}