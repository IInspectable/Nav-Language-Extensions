#region Using Directives

using System;

#endregion

namespace Pharmatechnik.Nav.Language.Text {

    public sealed class ClassifiedText {

        public ClassifiedText(string text, TextClassification classification) {
            Text           = text ?? throw new ArgumentNullException(nameof(text));
            Classification = classification;

        }

        public static readonly ClassifiedText Space = new ClassifiedText(" ", TextClassification.Whitespace);

        public static ClassifiedText Keyword(string keyword) => new ClassifiedText(keyword, TextClassification.Keyword);

        public static ClassifiedText TaskName(string taskName) => new ClassifiedText(taskName, TextClassification.TaskName);

        public string             Text           { get; }
        public TextClassification Classification { get; }

        public override string ToString() => Text;

    }

}