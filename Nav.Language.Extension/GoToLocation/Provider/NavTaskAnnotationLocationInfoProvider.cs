#region Using Directives

using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Pharmatechnik.Nav.Language.CodeAnalysis.Annotation;
using Pharmatechnik.Nav.Language.Extension.CSharp.GoTo;
using Pharmatechnik.Nav.Language.Extension.LanguageService;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.GoToLocation.Provider {

    class NavTaskAnnotationLocationInfoProvider: LocationInfoProvider {

        public NavTaskAnnotationLocationInfoProvider(NavTaskAnnotation navTaskAnnotation) {
            TaskAnnotation = navTaskAnnotation;
        }

        public NavTaskAnnotation TaskAnnotation { get; }
        
        public override async Task<IEnumerable<LocationInfo>> GetLocationsAsync(CancellationToken cancellationToken = default(CancellationToken)) {

            string sourceText;
            var textBuffer = NavLanguagePackage.GetOpenTextBufferForFile(TaskAnnotation.NavFileName);
            if (textBuffer != null) {
                sourceText = textBuffer.CurrentSnapshot.GetText();
            } else {
                sourceText = File.ReadAllText(TaskAnnotation.NavFileName);
            }

            var location = await LocationFinder.FindNavDefinitionLocationsAsync(sourceText, TaskAnnotation, cancellationToken)
                                               .ConfigureAwait(false);

            return location;
        }
    }
}
