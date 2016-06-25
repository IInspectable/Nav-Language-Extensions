#region Using Directives

using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;

using Pharmatechnik.Nav.Language.CodeAnalysis.Annotation;
using Pharmatechnik.Nav.Language.CodeAnalysis.FindSymbols;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.GoToLocation.Provider {

    class NavInitCallLocationInfoProvider: CodeAnalysisLocationInfoProvider {

        readonly NavInitCallAnnotation _callAnnotation;

        public NavInitCallLocationInfoProvider(ITextBuffer sourceBuffer, NavInitCallAnnotation callAnnotation): base(sourceBuffer) {
            _callAnnotation = callAnnotation;
        }

        static ImageMoniker ImageMoniker { get { return KnownMonikers.MethodPublic; } }

        protected override async Task<IEnumerable<LocationInfo>> GetLocationsAsync(Project project, CancellationToken cancellationToken) {

            try {
                var location = await LocationFinder.FindCallBeginLogicDeclarationLocationsAsync(
                    project           : project,
                    initCallAnnotation: _callAnnotation,
                    cancellationToken : cancellationToken).ConfigureAwait(false);

                var locationInfo = LocationInfo.FromLocation(
                    location    : location,
                    displayName : "Go To BeginLogic",
                    imageMoniker: ImageMoniker);

                return ToEnumerable(locationInfo);

            } catch(LocationNotFoundException ex) {
                return ToEnumerable(LocationInfo.FromError(ex, ImageMoniker));
            }            
        }
    }
}