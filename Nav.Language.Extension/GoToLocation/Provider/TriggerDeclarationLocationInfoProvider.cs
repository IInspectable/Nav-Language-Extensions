#region Using Directives

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Text;
using Pharmatechnik.Nav.Language.Extension.Common;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.GoToLocation.Provider {

    class TriggerDeclarationLocationInfoProvider: LocationInfoProvider {

        readonly string _fullyQualifiedWfsBaseName;
        readonly string _triggerMethodName;
        readonly ITextBuffer _sourceBuffer;

        public TriggerDeclarationLocationInfoProvider(ITextBuffer sourceBuffer, string fullyQualifiedWfsBaseName, string triggerMethodName) {

            _sourceBuffer              = sourceBuffer;
            _fullyQualifiedWfsBaseName = fullyQualifiedWfsBaseName;
            _triggerMethodName         = triggerMethodName;
        }

        public override async Task<IEnumerable<LocationInfo>> GetLocationsAsync(CancellationToken cancellationToken = default(CancellationToken)) {

            var project = _sourceBuffer.GetContainingProject();
            if (project == null) {
                // TODO Fehlermeldung
                return ToEnumerable(LocationInfo.FromError(""));
            }

            var location = await LocationFinder.FindTriggerLocationAsync(project, _fullyQualifiedWfsBaseName, _triggerMethodName, cancellationToken)
                                               .ConfigureAwait(false);

            return ToEnumerable(location);
        }
    }
}
