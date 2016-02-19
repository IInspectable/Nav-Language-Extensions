#region Using Directives

using System;
using System.Collections.Generic;

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Pharmatechnik.Nav.Language.Extension.Common;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Diagnostics {

    sealed class DiagnosticErrorTagger : SemanticModelServiceDependent, ITagger<DiagnosticErrorTag> {
        
        DiagnosticErrorTagger(ITextBuffer textBuffer): base(textBuffer) {
        }

        public static ITagger<T> GetOrCreateSingelton<T>(ITextBuffer textBuffer) where T : ITag {
            return new TextBufferScopedTagger<T>(
                textBuffer,
                typeof(DiagnosticErrorTagger), () =>
                   new DiagnosticErrorTagger(textBuffer) as ITagger<T>);
        }

        public IEnumerable<ITagSpan<DiagnosticErrorTag>> GetTags(NormalizedSnapshotSpanCollection spans) {

            var semanticModelResult = SemanticModelService.SemanticModelResult;
            if(semanticModelResult == null) {
                yield break;
            }

            var syntaxTree         = semanticModelResult.CodeGenerationUnit.Syntax.SyntaxTree;
            var codeGenerationUnit = semanticModelResult.CodeGenerationUnit;

            foreach (var span in spans) {

                //TODO: könnte evtl effektiver sein, wenn Errors nach Start sortiert sind.
                //==================
                // Syntax Fehler
                foreach (var diagnostic in syntaxTree.Diagnostics) {
                    if (diagnostic.Location.Start <= span.End && diagnostic.Location.End >= span.Start) {

                        var errorSpan = new SnapshotSpan(semanticModelResult.Snapshot, new Span(diagnostic.Location.Start, diagnostic.Location.Length));

                        var errorTag = new TagSpan<DiagnosticErrorTag>(
                                errorSpan.TranslateTo(span.Snapshot, SpanTrackingMode.EdgeExclusive),
                                new DiagnosticErrorTag(diagnostic));

                        yield return errorTag;
                    }
                }
                //==================
                // Semantic Fehler
                foreach (var diagnostic in codeGenerationUnit.Diagnostics) {
                    if (diagnostic.Location.Start <= span.End && diagnostic.Location.End >= span.Start) {
                
                        var errorSpan = new SnapshotSpan(semanticModelResult.Snapshot, new Span(diagnostic.Location.Start, diagnostic.Location.Length));
                
                        var errorTag = new TagSpan<DiagnosticErrorTag>(
                                errorSpan.TranslateTo(span.Snapshot, SpanTrackingMode.EdgeExclusive),
                                new DiagnosticErrorTag(diagnostic));

                        yield return errorTag;
                    }
                }
            }
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        protected override void OnSemanticModelChanged(object sender, SnapshotSpanEventArgs snapshotSpanEventArgs) {
            TagsChanged?.Invoke(this, snapshotSpanEventArgs);
        }       
    }
}