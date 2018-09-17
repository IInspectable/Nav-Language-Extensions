﻿#region Using Directives

using System;

#endregion

namespace Pharmatechnik.Nav.Language.Text {

    public sealed class ClassifiedText {

        public ClassifiedText(string text, TextClassification classification) {
            Text           = text ?? throw new ArgumentNullException(nameof(text));
            Classification = classification;

        }

        public string             Text           { get; }
        public TextClassification Classification { get; }

        public override string ToString() => Text;

    }

}