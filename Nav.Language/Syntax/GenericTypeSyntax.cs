using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Pharmatechnik.Nav.Language {
    [Serializable]
    [SampleSyntax("Type<T1, T2<T3, T4>>")]
    public partial class GenericTypeSyntax : CodeTypeSyntax {
        readonly IReadOnlyList<CodeTypeSyntax> _genericArguments;

        internal GenericTypeSyntax(TextExtent extent, IReadOnlyList<CodeTypeSyntax> genericArguments) : base(extent) {
            AddChildNodes(_genericArguments = genericArguments);
        }

        public SyntaxToken Identifier => ChildTokens().FirstOrMissing(SyntaxTokenType.Identifier);

        [NotNull]
        public IReadOnlyList<CodeTypeSyntax> GenericArguments => _genericArguments;
    }
}