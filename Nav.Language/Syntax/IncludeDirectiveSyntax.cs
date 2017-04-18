using System;

namespace Pharmatechnik.Nav.Language {

    [Serializable]
    [SampleSyntax("taskref \"file.nav\";")]
    public sealed partial class IncludeDirectiveSyntax : MemberDeclarationSyntax {

        internal IncludeDirectiveSyntax(TextExtent extent) : base(extent) {
        }

        public SyntaxToken TaskrefKeyword {
            get { return ChildTokens().FirstOrMissing(SyntaxTokenType.TaskrefKeyword); }
        }

        public SyntaxToken StringLiteral {
            get { return ChildTokens().FirstOrMissing(SyntaxTokenType.StringLiteral); }
        }

        public SyntaxToken Semicolon {
            get { return ChildTokens().FirstOrMissing(SyntaxTokenType.Semicolon); }
        }

        protected override bool PromiseNoDescendantNodeOfSameType {
            get { return true; }
        }
    }
}