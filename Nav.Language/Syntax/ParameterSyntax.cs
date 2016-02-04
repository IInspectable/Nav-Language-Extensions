using System;
using JetBrains.Annotations;

namespace Pharmatechnik.Nav.Language {
    [Serializable]
    [SampleSyntax("Type param")]
    public partial class ParameterSyntax : SyntaxNode {
        readonly CodeTypeSyntax _type;

        internal ParameterSyntax(TextExtent extent, CodeTypeSyntax type) : base(extent) {
            AddChildNode(_type = type);
        }
        
        [CanBeNull]
        public CodeTypeSyntax Type {
            get { return _type; }
        }

        public SyntaxToken Identifier {
            get { return ChildTokens().FirstOrMissing(SyntaxTokenType.Identifier); }
        }
    }
}