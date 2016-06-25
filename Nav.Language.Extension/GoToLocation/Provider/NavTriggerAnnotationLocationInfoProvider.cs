#region Using Directives

using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Pharmatechnik.Nav.Language.CodeAnalysis.Annotation;
using Pharmatechnik.Nav.Language.CodeAnalysis.FindSymbols;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.GoToLocation.Provider {

    class NavTriggerAnnotationLocationInfoProvider : NavAnnotationLocationInfoProvider<NavTriggerAnnotation> {

        public NavTriggerAnnotationLocationInfoProvider(NavTriggerAnnotation annotation) : base(annotation) {
        }

        protected override async Task<IEnumerable<LocationInfo>> GetLocationsAsync(string sourceText, CancellationToken cancellationToken = new CancellationToken()) {
            try {

                var locs = await LocationFinder.FindNavLocationsAsync(
                    sourceText       : sourceText,
                    annotation       : Annotation,
                    cancellationToken: cancellationToken).ConfigureAwait(false);

                return locs.Select(location => LocationInfo.FromLocation(
                    location   : location,
                    displayName: Annotation.TriggerName,
                    kind       : LocationKind.SignalTriggerDefinition));

            } catch (LocationNotFoundException ex) {
                return ToEnumerable(LocationInfo.FromError(ex));
            }
        }
    }
}