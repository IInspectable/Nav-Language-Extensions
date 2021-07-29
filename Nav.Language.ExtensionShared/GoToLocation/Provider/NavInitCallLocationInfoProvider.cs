#region Using Directives

using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Imaging.Interop;

using Pharmatechnik.Nav.Language.Extension.Images;
using Pharmatechnik.Nav.Language.CodeAnalysis.Annotation;
using Pharmatechnik.Nav.Language.CodeAnalysis.FindSymbols;

using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.GoToLocation.Provider {

    class NavInitCallLocationInfoProvider: CodeAnalysisLocationInfoProvider {

        readonly NavInitCallAnnotation _callAnnotation;

        [CanBeNull]
        readonly NavExitAnnotation _exitAnnotation;

        public NavInitCallLocationInfoProvider(ITextBuffer sourceBuffer,
                                               NavInitCallAnnotation callAnnotation,
                                               [CanBeNull] NavExitAnnotation exitAnnotation): base(sourceBuffer) {
            _callAnnotation = callAnnotation;
            _exitAnnotation = exitAnnotation;
        }

        static ImageMoniker ImageMoniker => ImageMonikers.GoToMethodPublic;

        protected override async Task<IEnumerable<LocationInfo>> GetLocationsAsync(Project project, CancellationToken cancellationToken) {

            LocationInfo beginLocationInfo;
            try {
                var location = await LocationFinder.FindCallBeginLogicDeclarationLocationsAsync(
                    project           : project,
                    initCallAnnotation: _callAnnotation,
                    cancellationToken : cancellationToken).ConfigureAwait(false);

                beginLocationInfo = LocationInfo.FromLocation(
                    location          : location,
                    displayName       : "BeginLogic",
                    imageMoniker      : ImageMoniker);

            } catch (LocationNotFoundException ex) {
                beginLocationInfo = LocationInfo.FromError(ex, ImageMoniker);
            }

            if (_exitAnnotation == null) {
                return ToEnumerable(beginLocationInfo);
            }

            var memberLocation = _exitAnnotation.MethodDeclarationSyntax.Identifier.GetLocation();
            var afterLocation  = LocationFinder.ToLocation(memberLocation);

            var afterLocationInfo = LocationInfo.FromLocation(
                location    : afterLocation,
                displayName : _exitAnnotation.MethodDeclarationSyntax.Identifier.Text,
                imageMoniker: ImageMoniker);

            return new[] {beginLocationInfo, afterLocationInfo};

        }

    }

}