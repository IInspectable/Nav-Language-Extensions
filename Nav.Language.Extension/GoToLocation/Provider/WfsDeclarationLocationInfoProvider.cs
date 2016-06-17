#region Using Directives

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Text;
using Pharmatechnik.Nav.Language.Extension.Common;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.GoToLocation.Provider {

    class WfsDeclarationLocationInfoProvider: LocationInfoProvider {

        readonly string _fullyQualifiedTypeName;
        readonly ITextBuffer _sourceBuffer;

        public WfsDeclarationLocationInfoProvider(ITextBuffer sourceBuffer, string fullyQualifiedTypeName) {

            _sourceBuffer           = sourceBuffer;
            _fullyQualifiedTypeName = fullyQualifiedTypeName;
        }

        public override async Task<IEnumerable<LocationInfo>> GetLocationsAsync(CancellationToken cancellationToken = default(CancellationToken)) {

            var project = _sourceBuffer.GetContainingProject();
            if (project == null) {
                // TODO Fehlermeldung
                return ToEnumerable(LocationInfo.FromError($"Das Projekt konnte nicht ermittelt werden."));
            }

            var location = await LocationFinder.FindWfsDeclarationAsync(project, _fullyQualifiedTypeName, cancellationToken)
                                               .ConfigureAwait(false);

            return ToEnumerable(location);
        }
    }
}
