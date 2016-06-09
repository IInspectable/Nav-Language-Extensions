#region Using Directives

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Pharmatechnik.Nav.Language.Extension.Common;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.GoTo {

    sealed class GoToTagger : SemanticModelServiceDependent, ITagger<GoToTag> {
        
        GoToTagger(ITextBuffer textBuffer) : base(textBuffer) {

        }

        public static ITagger<T> GetOrCreateSingelton<T>(ITextBuffer textBuffer) where T : ITag {
            return new TextBufferScopedTagger<T>(
                textBuffer,
                typeof(GoToTagger),
                () => new GoToTagger(textBuffer) as ITagger<T>);
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        protected override void OnSemanticModelChanged(object sender, SnapshotSpanEventArgs snapshotSpanEventArgs) {
            TagsChanged?.Invoke(this, snapshotSpanEventArgs);
        }

        public IEnumerable<ITagSpan<GoToTag>> GetTags(NormalizedSnapshotSpanCollection spans) {

            var semanticModelResult = SemanticModelService.SemanticModelResult;
            if (semanticModelResult == null) {
                yield break;
            }
            
            foreach (var span in spans) {
                
                var extent  = TextExtent.FromBounds(span.Start, span.End);
                var symbols = semanticModelResult.CodeGenerationUnit.Symbols[extent, includeOverlapping: true];

                foreach (var symbol in symbols) {

                    var navigateToTag = GoToSymbolBuilder.Build(semanticModelResult, symbol, TextBuffer);
                    if(navigateToTag != null) {
                        yield return navigateToTag;
                    }
                }
            }
        }
    }
}