using System;
using JetBrains.Annotations;

using Pharmatechnik.Nav.Language.Text;

namespace Pharmatechnik.Nav.Language {
    [Serializable]
    [SampleSyntax("[using Namespace]")]
    public sealed partial class CodeUsingDeclarationSyntax : CodeSyntax {
        readonly IdentifierOrStringSyntax _namespace;
        
        internal CodeUsingDeclarationSyntax(TextExtent extent, IdentifierOrStringSyntax namespaceSyntax) : base(extent) {
            AddChildNode(_namespace = namespaceSyntax);
        }

        public SyntaxToken UsingKeyword => ChildTokens().FirstOrMissing(SyntaxTokenType.UsingKeyword);

        [CanBeNull]
        public IdentifierOrStringSyntax Namespace => _namespace;

        private protected override bool PromiseNoDescendantNodeOfSameType => true;
    }
}
