#region Using Directives

using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using Pharmatechnik.Nav.Language.CodeAnalysis.Annotation;
using Pharmatechnik.Nav.Language.CodeAnalysis.FindSymbols;
using Pharmatechnik.Nav.Language.Extension.LanguageService;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.GoToLocation.Provider {

    abstract class NavAnnotationLocationInfoProvider<TAnnotation> : LocationInfoProvider 
        where TAnnotation: NavTaskAnnotation {
        
        protected NavAnnotationLocationInfoProvider(TAnnotation annotation) {
            Annotation = annotation;
        }
        
        public TAnnotation Annotation { get; set; }

        public override async Task<IEnumerable<LocationInfo>> GetLocationsAsync(CancellationToken cancellationToken = default(CancellationToken)) {

            string sourceText;
            var textBuffer = NavLanguagePackage.GetOpenTextBufferForFile(Annotation.NavFileName);
            if (textBuffer != null) {
                sourceText = textBuffer.CurrentSnapshot.GetText();
            } else {
                sourceText = File.ReadAllText(Annotation.NavFileName);
            }

            var location = await LocationFinder.FindNavDefinitionLocationsAsync(sourceText, Annotation, cancellationToken)
                .ConfigureAwait(false);

            return location;
        }

        protected abstract Task<IEnumerable<LocationInfo>> GetLocationsAsync(string sourceText, CancellationToken cancellationToken = default(CancellationToken));
    }
}