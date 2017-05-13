using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;

namespace Pharmatechnik.Nav.Language.Extension.Outlining {

    class TaskDefinitionsOutlineTagger {

        public static IEnumerable<ITagSpan<IOutliningRegionTag>> GetTags(SyntaxTreeAndSnapshot syntaxTreeAndSnapshot, IOutliningRegionTagCreator tagCreator) {

            foreach (var taskDef in syntaxTreeAndSnapshot.SyntaxTree.GetRoot().DescendantNodes<TaskDefinitionSyntax>()) {

                var extent = taskDef.Extent;

                var nameToken = taskDef.Identifier;
                if (nameToken.IsMissing) {
                    continue;
                }

                var rgnStartToken = nameToken.NextToken();
                if (rgnStartToken.IsMissing) {
                    continue;
                }

                var start  = Math.Min(nameToken.End + 1, rgnStartToken.Start);
                int length = extent.End - start;

                if (length <= 0) {
                    continue;
                }

                var regionSpan = new SnapshotSpan(new SnapshotPoint(syntaxTreeAndSnapshot.Snapshot, start), length);
                var hintSpan   = new SnapshotSpan(new SnapshotPoint(syntaxTreeAndSnapshot.Snapshot, extent.Start), extent.Length);
                var tag        = tagCreator.CreateTag("...", hintSpan);

                yield return new TagSpan<IOutliningRegionTag>(regionSpan, tag);
            }
        }
    }
}