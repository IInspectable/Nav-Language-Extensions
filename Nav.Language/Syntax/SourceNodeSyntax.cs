using System;

namespace Pharmatechnik.Nav.Language {
    [Serializable]
    public abstract class SourceNodeSyntax : SyntaxNode {

        protected SourceNodeSyntax(TextExtent extent) : base(extent) {
        }

        public abstract string Name { get; }
    }

    [Serializable]
    [SampleSyntax("init")]
    public partial class InitSourceNodeSyntax : SourceNodeSyntax {

        internal InitSourceNodeSyntax(TextExtent extent) : base(extent) {
        }

        public SyntaxToken InitKeyword {
            get { return ChildTokens().FirstOrMissing(SyntaxTokenType.InitKeyword); }
        }

        public override string Name {
            get { return InitKeyword.ToString(); }
        }

    }

    [Serializable]
    [SampleSyntax("Identifier")]
    public partial class IdentifierSourceNodeSyntax : SourceNodeSyntax {

        internal IdentifierSourceNodeSyntax(TextExtent extent) : base(extent) {
        }

        public SyntaxToken Identifier {
            get { return ChildTokens().FirstOrMissing(SyntaxTokenType.Identifier); }
        }

        public override string Name
        {
            get { return Identifier.ToString(); }
        }
    }
}