#region Using Directives

using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.Text;

using Pharmatechnik.Nav.Language.Extension.Common;
using Pharmatechnik.Nav.Language.CodeAnalysis.FindSymbols;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.GoToLocation.Provider {

    abstract class CodeAnalysisLocationInfoProvider: LocationInfoProvider {
        readonly ITextBuffer _sourceBuffer;

        protected CodeAnalysisLocationInfoProvider(ITextBuffer sourceBuffer) {
            _sourceBuffer = sourceBuffer;
        }
         
        public sealed override async Task<IEnumerable<LocationInfo>> GetLocationsAsync(CancellationToken cancellationToken = new CancellationToken()) {
            var project = _sourceBuffer.GetContainingProject();
            if (project == null) {
                // TODO Fehlermeldung
                return ToEnumerable(LocationInfo.FromError("Unable to determine containing project."));
            }

            return await GetLocationsAsync(project, cancellationToken);
        }

        protected abstract Task<IEnumerable<LocationInfo>> GetLocationsAsync(Project project, CancellationToken cancellationToken);
    }
}