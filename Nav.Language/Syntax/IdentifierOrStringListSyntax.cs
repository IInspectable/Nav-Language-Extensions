using System;
using System.Collections;
using System.Collections.Generic;

namespace Pharmatechnik.Nav.Language {
    [Serializable]
    [SampleSyntax("Identifier, \"StringLiteral\"")]
    public sealed partial class IdentifierOrStringListSyntax : SyntaxNode, IReadOnlyList<IdentifierOrStringSyntax> {

        readonly IReadOnlyList<IdentifierOrStringSyntax> _identifierOrStrings;

        internal IdentifierOrStringListSyntax(TextExtent extent, 
            IReadOnlyList<IdentifierOrStringSyntax> identifierOrStrings) : base(extent) {
            AddChildNodes(_identifierOrStrings = identifierOrStrings);
        }

        public IEnumerator<IdentifierOrStringSyntax> GetEnumerator() {
            return _identifierOrStrings.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public int Count => _identifierOrStrings.Count;

        public IdentifierOrStringSyntax this[int index] => _identifierOrStrings[index];
    }
}