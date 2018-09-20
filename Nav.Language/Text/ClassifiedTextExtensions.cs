#region Using Directives

using System.Linq;
using System.Text;
using System.Collections.Generic;

#endregion

namespace Pharmatechnik.Nav.Language.Text {

    public static class ClassifiedTextExtensions {

        public static string JoinText(this IEnumerable<ClassifiedText> parts) {
            return parts.Aggregate(new StringBuilder(), (sb, p) => sb.Append(p.Text), sb => sb.ToString());
        }

        public static int Length(this IEnumerable<ClassifiedText> parts) {
            return parts.Aggregate(0, (acc, ct) => acc + ct.Text.Length);
        }

        public static IEnumerable<ClassifiedText> GetClassifiedText(this SyntaxTree syntaxTree, TextExtent extent) {

            foreach (var token in syntaxTree.Tokens[extent, includeOverlapping: true]) {
                yield return new ClassifiedText(ToString(token, extent), token.Classification);

            }

        }

        static string ToString(SyntaxToken token, TextExtent extent) {

            int startOffset = 0;
            if (extent.Start > token.Start) {
                startOffset = extent.Start - token.Start;
            }

            int endOffset = 0;
            if (extent.End < token.End) {
                endOffset = token.End - extent.End;
            }

            var text = token.ToString();

            if (startOffset == 0 && endOffset == 0) {
                return text;
            }

            return text.Substring(startOffset, text.Length - startOffset - endOffset);
        }

    }

}