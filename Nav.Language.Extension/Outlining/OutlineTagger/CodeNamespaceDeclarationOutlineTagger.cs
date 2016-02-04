using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;

namespace Pharmatechnik.Nav.Language.Extension.Outlining {

    class CodeNamespaceDeclarationOutlineTagger {

        public static IEnumerable<ITagSpan<IOutliningRegionTag>> GetTags(ParseResult parseResult, IOutliningRegionTagCreator tagCreator) {
            
            var nsDecl = parseResult.SyntaxTree.GetRoot().DescendantNodes<CodeNamespaceDeclarationSyntax>().FirstOrDefault();
            if (nsDecl == null) {
                yield break;
            }

            var keywordToken = nsDecl.NamespaceprefixKeyword;
            if (keywordToken.IsMissing) {
                yield break;
            }

            var start  = keywordToken.End + 1;
            int length = parseResult.Snapshot.Length - start; // Bis zum Ende der Datei

            if (length <= 0) {
                yield break;
            }

            var span = new SnapshotSpan(new SnapshotPoint(parseResult.Snapshot, start), length);
            var tag  = tagCreator.CreateTag("...", span);

            yield return new TagSpan<IOutliningRegionTag>(span, tag);
        }
    }
}