using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;

namespace Pharmatechnik.Nav.Language.Extension.Outlining {

    class MultilineCommentOutlineTagger {

        public static IEnumerable<ITagSpan<IOutliningRegionTag>> GetTags(ParseResult parseResult, IOutliningRegionTagCreator tagCreator) {

            foreach(var mc in parseResult.SyntaxTree.Tokens.OfType(SyntaxTokenType.MultiLineComment)) {
                var extent = mc.Extent;

                if (extent.IsEmptyOrMissing) {
                    continue;
                }

                var startLine = parseResult.Snapshot.GetLineNumberFromPosition(extent.Start);
                var endLine   = parseResult.Snapshot.GetLineNumberFromPosition(extent.End);
                if (startLine == endLine) {
                    continue;
                }

                var collapsedForm = "/* ...";
                var rgnSpan  = new SnapshotSpan(new SnapshotPoint(parseResult.Snapshot, extent.Start), extent.Length);
                var hintSpan = new SnapshotSpan(new SnapshotPoint(parseResult.Snapshot, extent.Start), extent.Length);
                var rgnTag = tagCreator.CreateTag(collapsedForm, hintSpan);

                yield return new TagSpan<IOutliningRegionTag>(rgnSpan, rgnTag);
            }           
        }
    }
}