#region Using Directives

using System;
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
            if(annotation == null) {
                throw new ArgumentNullException(nameof(annotation));
            }
            Annotation = annotation;
        }
        
        public TAnnotation Annotation { get; set; }

        public sealed override async Task<IEnumerable<LocationInfo>> GetLocationsAsync(CancellationToken cancellationToken = default(CancellationToken)) {

            string sourceText;
            var textBuffer = NavLanguagePackage.GetOpenTextBufferForFile(Annotation.NavFileName);
            if (textBuffer != null) {
                sourceText = textBuffer.CurrentSnapshot.GetText();
            } else {
                sourceText = await Task.Run(()=> File.ReadAllText(Annotation.NavFileName), cancellationToken).ConfigureAwait(false);
            }

            return await GetLocationsAsync(sourceText, cancellationToken).ConfigureAwait(false);
        }

        protected abstract Task<IEnumerable<LocationInfo>> GetLocationsAsync(string sourceText, CancellationToken cancellationToken = default(CancellationToken));
    }
}