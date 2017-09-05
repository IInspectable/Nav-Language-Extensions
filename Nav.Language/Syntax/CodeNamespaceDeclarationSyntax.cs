using System;
using JetBrains.Annotations;

namespace Pharmatechnik.Nav.Language {
    [Serializable]
    [SampleSyntax("[namespaceprefix Namespace]")]
    public sealed partial class CodeNamespaceDeclarationSyntax : CodeSyntax {

        readonly IdentifierOrStringSyntax _namespace;

        internal CodeNamespaceDeclarationSyntax(TextExtent extent, IdentifierOrStringSyntax namespaceSyntax)
                : base(extent) {

            AddChildNode(_namespace = namespaceSyntax);
        }

        public SyntaxToken NamespaceprefixKeyword {
            get { return ChildTokens().FirstOrMissing(SyntaxTokenType.NamespaceprefixKeyword); }
        }

        [CanBeNull]
        public IdentifierOrStringSyntax Namespace {
            get { return _namespace; }
        }

        protected override bool PromiseNoDescendantNodeOfSameType {
            get { return true; }
        }
    }
}
