#region Using Directives

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Text;
using Pharmatechnik.Nav.Language.CodeAnalysis.Annotation;
using Pharmatechnik.Nav.Language.CodeAnalysis.FindSymbols;
using Pharmatechnik.Nav.Language.Extension.Common;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.GoToLocation.Provider {

    class NavInitCallLocationInfoProvider: LocationInfoProvider {

        readonly ITextBuffer _sourceBuffer;
        readonly NavInitCallAnnotation _callAnnotation;

        public NavInitCallLocationInfoProvider(ITextBuffer sourceBuffer, NavInitCallAnnotation callAnnotation) {
            _sourceBuffer   = sourceBuffer;
            _callAnnotation = callAnnotation;
        }

        public override async Task<IEnumerable<LocationInfo>> GetLocationsAsync(CancellationToken cancellationToken = new CancellationToken()) {

            var project = _sourceBuffer.GetContainingProject();
            if (project == null) {
                // TODO Fehlermeldung
                return ToEnumerable(LocationInfo.FromError(""));
            }

            var location = await LocationFinder.FindCallBeginLogicDeclarationLocationsAsync(
                project            : project,
                initCallAnnotation : _callAnnotation,
                cancellationToken  : cancellationToken).ConfigureAwait(false);

            return ToEnumerable(location);
        }
    }
}
