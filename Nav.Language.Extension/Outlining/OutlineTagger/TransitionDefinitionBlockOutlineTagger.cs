using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;

namespace Pharmatechnik.Nav.Language.Extension.Outlining {

    class TransitionDefinitionBlockOutlineTagger {

        public static IEnumerable<ITagSpan<IOutliningRegionTag>> GetTags(ParseResult parseResult, IOutliningRegionTagCreator tagCreator) {

            var transitionBlocks = parseResult.SyntaxTree.GetRoot().DescendantNodes().OfType<TransitionDefinitionBlockSyntax>();

            foreach (var transitionBlock in transitionBlocks) {
                var extent = transitionBlock.Extent;
                
                if (extent.IsEmptyOrMissing) {
                    continue;
                }

                var startLine = parseResult.Snapshot.GetLineNumberFromPosition(extent.Start);
                var endLine = parseResult.Snapshot.GetLineNumberFromPosition(extent.End);
                if (startLine == endLine) {
                    continue;
                }

                var collapsedForm = "Transitions";
                var rgnSpan  = new SnapshotSpan(new SnapshotPoint(parseResult.Snapshot, extent.Start), extent.Length);
                var hintSpan = new SnapshotSpan(new SnapshotPoint(parseResult.Snapshot, extent.Start), extent.Length);
                var rgnTag   = tagCreator.CreateTag(collapsedForm, hintSpan);

                yield return new TagSpan<IOutliningRegionTag>(rgnSpan, rgnTag);
            }
        }

    }

}