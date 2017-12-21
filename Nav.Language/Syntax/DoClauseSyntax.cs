using System;
using JetBrains.Annotations;

namespace Pharmatechnik.Nav.Language {
    [Serializable]
    [SampleSyntax("do \"instruction\"")]
    public partial class DoClauseSyntax : SyntaxNode {
        readonly IdentifierOrStringSyntax _identifierOrString;

        internal DoClauseSyntax(TextExtent extent, IdentifierOrStringSyntax identifierOrString) : base(extent) {
            AddChildNode(_identifierOrString = identifierOrString);
        }

        public SyntaxToken DoKeyword => ChildTokens().FirstOrMissing(SyntaxTokenType.DoKeyword);

        [CanBeNull]
        public IdentifierOrStringSyntax IdentifierOrString => _identifierOrString;
    }
}