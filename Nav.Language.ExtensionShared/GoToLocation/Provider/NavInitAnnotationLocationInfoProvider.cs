#region Using Directives

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.VisualStudio.Imaging.Interop;
using Pharmatechnik.Nav.Language.CodeAnalysis.Annotation;
using Pharmatechnik.Nav.Language.CodeAnalysis.FindSymbols;
using Pharmatechnik.Nav.Language.Extension.Images;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.GoToLocation.Provider {

    class NavInitAnnotationLocationInfoProvider : NavAnnotationLocationInfoProvider<NavInitAnnotation> {

        public NavInitAnnotationLocationInfoProvider(NavInitAnnotation annotation) : base(annotation) {
        }

        static ImageMoniker ImageMoniker { get { return ImageMonikers.InitConnectionPoint; } }

        protected override async Task<IEnumerable<LocationInfo>> GetLocationsAsync(string sourceText, CancellationToken cancellationToken = new CancellationToken()) {

            try {

                var locs = await LocationFinder.FindNavLocationsAsync(
                    sourceText       : sourceText,
                    annotation       : Annotation,
                    cancellationToken: cancellationToken).ConfigureAwait(false);

                return locs.Select(location => LocationInfo.FromLocation(
                    location    : location,
                    displayName : Annotation.InitName,
                    imageMoniker: ImageMoniker));

            } catch (LocationNotFoundException ex) {
                return ToEnumerable(LocationInfo.FromError(ex, ImageMoniker));
            }
        }
    }
}