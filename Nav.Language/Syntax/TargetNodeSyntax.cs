using System;
using JetBrains.Annotations;

namespace Pharmatechnik.Nav.Language {

    [Serializable]
    public abstract class TargetNodeSyntax : SyntaxNode {
        protected TargetNodeSyntax(TextExtent extent) : base(extent) {}
        public abstract string Name { get; }
    }

    [Serializable]
    [SampleSyntax("end")]
    public partial class EndTargetNodeSyntax : TargetNodeSyntax {
        internal EndTargetNodeSyntax(TextExtent extent) : base(extent) {}

        public SyntaxToken EndKeyword {
            get { return ChildTokens().FirstOrMissing(SyntaxTokenType.EndKeyword); }
        }

        public override string Name {
            get { return EndKeyword.ToString(); }
        }

    }

    [Serializable]
    [SampleSyntax("Identifier (identifierOrStringList)")]
    public partial class IdentifierTargetNodeSyntax : TargetNodeSyntax {
        readonly IdentifierOrStringListSyntax _identifierOrStringList;

        internal IdentifierTargetNodeSyntax(TextExtent extent, IdentifierOrStringListSyntax identifierOrStringList) 
            : base(extent) {
            AddChildNode(_identifierOrStringList = identifierOrStringList);
        }        

        public SyntaxToken Identifier {
            get { return ChildTokens().FirstOrMissing(SyntaxTokenType.Identifier); }
        }

        public override string Name {
            get { return Identifier.ToString(); }
        }

        [CanBeNull]
        public IdentifierOrStringListSyntax IdentifierOrStringList {
            get { return _identifierOrStringList; }
        }
    }
}