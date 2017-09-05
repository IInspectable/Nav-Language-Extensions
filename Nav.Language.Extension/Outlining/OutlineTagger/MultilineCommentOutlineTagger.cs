using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;

namespace Pharmatechnik.Nav.Language.Extension.Outlining {

    class MultilineCommentOutlineTagger {

        public static IEnumerable<ITagSpan<IOutliningRegionTag>> GetTags(SyntaxTreeAndSnapshot syntaxTreeAndSnapshot, IOutliningRegionTagCreator tagCreator) {

            foreach(var mc in syntaxTreeAndSnapshot.SyntaxTree.Tokens.OfType(SyntaxTokenType.MultiLineComment)) {
                var extent = mc.Extent;

                if (extent.IsEmptyOrMissing) {
                    continue;
                }

                var startLine = syntaxTreeAndSnapshot.Snapshot.GetLineNumberFromPosition(extent.Start);
                var endLine   = syntaxTreeAndSnapshot.Snapshot.GetLineNumberFromPosition(extent.End);
                if (startLine == endLine) {
                    continue;
                }

                var collapsedForm = "/* ...";
                var rgnSpan  = new SnapshotSpan(new SnapshotPoint(syntaxTreeAndSnapshot.Snapshot, extent.Start), extent.Length);
                var hintSpan = new SnapshotSpan(new SnapshotPoint(syntaxTreeAndSnapshot.Snapshot, extent.Start), extent.Length);
                var rgnTag = tagCreator.CreateTag(collapsedForm, hintSpan);

                yield return new TagSpan<IOutliningRegionTag>(rgnSpan, rgnTag);
            }           
        }
    }
}