using System;
using Pharmatechnik.Nav.Language.Internal;

namespace Pharmatechnik.Nav.Language {

    public enum EdgeMode {
        Modal,
        NonModal,
        Goto
    }

    [Serializable]
    public abstract class EdgeSyntax : SyntaxNode {

        protected EdgeSyntax(TextExtent extent) : base(extent) {}

        public abstract SyntaxToken Keyword { get; }
        public abstract EdgeMode Mode { get; }
    }

    [Serializable]
    [SampleSyntax("o->")]
    public partial class ModalEdgeSyntax : EdgeSyntax {
        internal ModalEdgeSyntax(TextExtent extent) : base(extent) {}

        [SuppressCodeSanityCheck("Der Name Keyword ist hier ausdrücklich gewollt.")]
        public override SyntaxToken Keyword {
            get { return ModalEdgeKeyword; }
        }

        public SyntaxToken ModalEdgeKeyword {
            get { return ChildTokens().FirstOrMissing(SyntaxTokenType.ModalEdgeKeyword); }
        }

        public override EdgeMode Mode {
            get { return EdgeMode.Modal; }
        }
    }

    [Serializable]
    [SampleSyntax("==>")]
    public partial class NonModalEdgeSyntax : EdgeSyntax {
        internal NonModalEdgeSyntax(TextExtent extent) : base(extent) {}

        [SuppressCodeSanityCheck("Der Name Keyword ist hier ausdrücklich gewollt.")]
        public override SyntaxToken Keyword {
            get { return NonModalEdgeKeyword; }
        }

        public SyntaxToken NonModalEdgeKeyword {
            get { return ChildTokens().FirstOrMissing(SyntaxTokenType.NonModalEdgeKeyword); }
        }

        public override EdgeMode Mode {
            get { return EdgeMode.NonModal; }
        }
    }

    [Serializable]
    [SampleSyntax("-->")]
    public partial class GoToEdgeSyntax : EdgeSyntax {

        internal GoToEdgeSyntax(TextExtent extent) : base(extent) {}

        [SuppressCodeSanityCheck("Der Name Keyword ist hier ausdrücklich gewollt.")]
        public override SyntaxToken Keyword {
            get { return GoToEdgeKeyword; }
        }

        public SyntaxToken GoToEdgeKeyword {
            get { return ChildTokens().FirstOrMissing(SyntaxTokenType.GoToEdgeKeyword); }
        }

        public override EdgeMode Mode {
            get { return EdgeMode.Goto; }
        }
    }
}