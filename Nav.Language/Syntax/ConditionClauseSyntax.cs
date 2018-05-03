using System;
using JetBrains.Annotations;

using Pharmatechnik.Nav.Language.Text;

namespace Pharmatechnik.Nav.Language {

    [Serializable]
    public abstract class ConditionClauseSyntax : SyntaxNode {
        protected ConditionClauseSyntax(TextExtent extent) : base(extent) {}
    }

    [Serializable]
    [SampleSyntax("if Condition")]
    public partial class IfConditionClauseSyntax : ConditionClauseSyntax {
        readonly IdentifierOrStringSyntax _identifierOrString;

        internal IfConditionClauseSyntax(TextExtent extent, IdentifierOrStringSyntax identifierOrString) : base(extent) {
            AddChildNode(_identifierOrString = identifierOrString);
        }

        public SyntaxToken IfKeyword => ChildTokens().FirstOrMissing(SyntaxTokenType.IfKeyword);

        [CanBeNull]
        public IdentifierOrStringSyntax IdentifierOrString => _identifierOrString;
    }

    [Serializable]
    [SampleSyntax("else")]
    public partial class ElseConditionClauseSyntax : ConditionClauseSyntax {

        internal ElseConditionClauseSyntax(TextExtent extent) : base(extent) {
        }

        public SyntaxToken ElseKeyword => ChildTokens().FirstOrMissing(SyntaxTokenType.ElseKeyword);
    }

    [Serializable]
    [SampleSyntax("else if Condition")]
    public partial class ElseIfConditionClauseSyntax : ConditionClauseSyntax {
        readonly ElseConditionClauseSyntax _elseCondition;
        readonly IfConditionClauseSyntax   _ifCondition;

        internal ElseIfConditionClauseSyntax(TextExtent extent, ElseConditionClauseSyntax elseCondition, IfConditionClauseSyntax ifCondition) : base(extent) {
            AddChildNode(_elseCondition = elseCondition);
            AddChildNode(_ifCondition   = ifCondition);
        }
       
        [NotNull]
        public ElseConditionClauseSyntax ElseCondition => _elseCondition;

        [NotNull]
        public IfConditionClauseSyntax IfCondition => _ifCondition;
    }
}