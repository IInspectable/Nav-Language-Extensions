using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;

namespace Pharmatechnik.Nav.Language.Extension.Outlining {

    class TaskReferenceOutlineTagger {

        public static IEnumerable<ITagSpan<IOutliningRegionTag>> GetTags(ParseResult parseResult, IOutliningRegionTagCreator tagCreator) {

            // Task Declarations
            foreach (var taskReferenceDefinition in parseResult.SyntaxTree.GetRoot().DescendantNodes().OfType<TaskDeclarationSyntax>()) {
                var extent = taskReferenceDefinition.Extent;
               
                if (extent.Length <= 0) {
                    continue;
                }

                var nameToken = taskReferenceDefinition.Identifier;

                if (nameToken.IsMissing) {
                    continue;
                }

                var rgnStartToken = nameToken.NextToken();
                if (rgnStartToken.IsMissing) {
                    continue;
                }

                int start  = Math.Min(nameToken.End+1, rgnStartToken.Start);
                int length = extent.End - start;

                if (length <= 0) {
                    continue;
                }

                var rgnSpan  = new SnapshotSpan(new SnapshotPoint(parseResult.Snapshot, start), length);
                var hintSpan = new SnapshotSpan(new SnapshotPoint(parseResult.Snapshot, extent.Start), extent.Length);
                var rgnTag   = tagCreator.CreateTag("...", hintSpan);

                yield return new TagSpan<IOutliningRegionTag>(rgnSpan, rgnTag);
            }


            // Zusammenhängende Blöcke von taskref "file" als Region zusammenfassen
            var allRelevant = parseResult.SyntaxTree.GetRoot().DescendantNodes<TaskDefinitionSyntax>().Concat<SyntaxNode>(
                parseResult.SyntaxTree.GetRoot().DescendantNodes <TaskDeclarationSyntax>())
                .Concat(
                    parseResult.SyntaxTree.GetRoot().DescendantNodes<IncludeDirectiveSyntax>())
                .OrderBy(s => s.Extent.Start);

            IncludeDirectiveSyntax firstInclude = null;
            IncludeDirectiveSyntax lastInclude  = null;
            foreach (var syntaxNode in allRelevant) {

                var include = syntaxNode as IncludeDirectiveSyntax;
                if (include != null) {
                    if (firstInclude == null) {
                        firstInclude = include;
                    }
                    lastInclude = include;
                } else {
                    if (firstInclude != null && firstInclude != lastInclude) {

                        var extendStart = firstInclude.Extent;
                        var extendEnd   = lastInclude.Extent;

                        var keywordToken = firstInclude.TaskrefKeyword;
                        
                        if (keywordToken.IsMissing) {
                            yield break;
                        }

                        int start = keywordToken.End + 1;

                        int length = extendEnd.End - start;

                        var regionSpan = new SnapshotSpan(new SnapshotPoint(parseResult.Snapshot, start), length);
                        var hintSpan   = new SnapshotSpan(new SnapshotPoint(parseResult.Snapshot, extendStart.Start), extendEnd.End - extendStart.Start);
                        var tag        = tagCreator.CreateTag("...", hintSpan);

                        yield return new TagSpan<IOutliningRegionTag>(regionSpan, tag);
                    }
                    firstInclude = lastInclude = null;
                }
            }
        }
    }
}