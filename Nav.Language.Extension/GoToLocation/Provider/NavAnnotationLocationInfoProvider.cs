#region Using Directives

using System;
using System.IO;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using Pharmatechnik.Nav.Language.CodeAnalysis.Annotation;
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
                try {
                    sourceText = await Task.Run(() => File.ReadAllText(Annotation.NavFileName), cancellationToken).ConfigureAwait(false);
                } catch(Exception ex) when (
                    ex is FileNotFoundException || 
                    ex is IOException ||
                    ex is UnauthorizedAccessException || 
                    ex is SecurityException) {
                    // TODO evtl. detaliertere Fehlermeldungen
                    return ToEnumerable(LocationInfo.FromError($"File '{Annotation.NavFileName}' not found"));
                }
            }

            return await GetLocationsAsync(sourceText, cancellationToken).ConfigureAwait(false);
        }

        protected abstract Task<IEnumerable<LocationInfo>> GetLocationsAsync(string sourceText, CancellationToken cancellationToken = default(CancellationToken));
    }
}