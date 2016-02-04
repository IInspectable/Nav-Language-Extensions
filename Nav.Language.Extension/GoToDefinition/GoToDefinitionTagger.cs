#region Using Directives

using System;
using System.Collections.Generic;

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Pharmatechnik.Nav.Language.Extension.Common;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.GoToDefinition {

    sealed class GoToDefinitionTagger : SemanticModelServiceDependent, ITagger<GoToDefinitionTag> {
        
        GoToDefinitionTagger(ITextBuffer textBuffer) : base(textBuffer) {

        }

        public static ITagger<T> GetOrCreateSingelton<T>(ITextBuffer textBuffer) where T : ITag {
            return new TextBufferScopedTagger<T>(
                textBuffer,
                typeof(GoToDefinitionTagger),
                () => new GoToDefinitionTagger(textBuffer) as ITagger<T>);
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        protected override void OnSemanticModelChanged(object sender, SnapshotSpanEventArgs snapshotSpanEventArgs) {
            TagsChanged?.Invoke(this, snapshotSpanEventArgs);
        }

        public IEnumerable<ITagSpan<GoToDefinitionTag>> GetTags(NormalizedSnapshotSpanCollection spans) {

            var semanticModelResult = SemanticModelService.SemanticModelResult;
            if (semanticModelResult == null) {
                yield break;
            }

            foreach (var span in spans) {
                
                var extent  = TextExtent.FromBounds(span.Start, span.End);
                var symbols = semanticModelResult.CompilationUnit.Symbols[extent, includeOverlapping: true];

                foreach (var symbol in symbols) {

                    var navigateToTag = GoToDefinitionSymbolBuilder.Build(semanticModelResult, symbol);
                    if(navigateToTag != null) {
                        yield return navigateToTag;
                    }
                }
            }
        }
    }
}