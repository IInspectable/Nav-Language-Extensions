using System;
using JetBrains.Annotations;

namespace Pharmatechnik.Nav.Language {
    [Serializable]
    [SampleSyntax("[using Namespace]")]
    public sealed partial class CodeUsingDeclarationSyntax : CodeSyntax {
        readonly IdentifierOrStringSyntax _namespace;
        
        internal CodeUsingDeclarationSyntax(TextExtent extent, IdentifierOrStringSyntax namespaceSyntax) : base(extent) {
            AddChildNode(_namespace = namespaceSyntax);
        }

        public SyntaxToken UsingKeyword {
            get { return ChildTokens().FirstOrMissing(SyntaxTokenType.UsingKeyword); }
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
