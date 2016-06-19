#region Using Directives

using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using Pharmatechnik.Nav.Language.CodeAnalysis.Annotation;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.GoToLocation.Provider {

    class NavExitAnnotationLocationInfoProvider : NavAnnotationLocationInfoProvider<NavExitAnnotation> {

        public NavExitAnnotationLocationInfoProvider(NavExitAnnotation annotation) : base(annotation) {
        }

        protected override Task<IEnumerable<LocationInfo>> GetLocationsAsync(string sourceText, CancellationToken cancellationToken = new CancellationToken()) {
            return LocationFinder.FindNavLocationsAsync(sourceText, Annotation, cancellationToken);
        }
    }
}