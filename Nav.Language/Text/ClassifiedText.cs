#region Using Directives

using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

#endregion

namespace Pharmatechnik.Nav.Language.Text {

    public sealed class ClassifiedText {

        public ClassifiedText(string text, TextClassification classification) {
            Text           = text ?? throw new ArgumentNullException(nameof(text));
            Classification = classification;

        }

        public static readonly ClassifiedText Space = new ClassifiedText(" ", TextClassification.Whitespace);
        public static ClassifiedText Keyword(string keyword) => new ClassifiedText(keyword,    TextClassification.Keyword);
        public static ClassifiedText TaskName(string taskName) => new ClassifiedText(taskName, TextClassification.TaskName);

        public string             Text           { get; }
        public TextClassification Classification { get; }

        public override string ToString() => Text;

    }

    public static class ClassifiedTextExtensions {

        public static string JoinText(this IEnumerable<ClassifiedText> parts) {
            return parts.Aggregate(new StringBuilder(), (sb, p) => sb.Append(p.Text), sb => sb.ToString());
        }

    }

}