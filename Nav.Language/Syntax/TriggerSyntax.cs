using System;
using JetBrains.Annotations;

using Pharmatechnik.Nav.Language.Text;

namespace Pharmatechnik.Nav.Language {

    [Serializable]
    public abstract class TriggerSyntax : SyntaxNode {
        protected TriggerSyntax(TextExtent extent) : base(extent) {}       
    }

    [Serializable]
    [SampleSyntax("spontaneous")]
    public partial class SpontaneousTriggerSyntax : TriggerSyntax {

        internal SpontaneousTriggerSyntax(TextExtent extent) : base(extent) {}
        
        public SyntaxToken SpontaneousKeyword => ChildTokens().FirstOrMissing(SyntaxTokenType.SpontaneousKeyword);

        public const string Keyword = "spontaneous";
    }
    
    [Serializable]
    [SampleSyntax("on Trigger")]
    public partial class SignalTriggerSyntax : TriggerSyntax {

        readonly IdentifierOrStringListSyntax _identifierOrStringList;

        internal SignalTriggerSyntax(TextExtent extent, IdentifierOrStringListSyntax identifierOrStringList) 
            : base(extent) {
            AddChildNode(_identifierOrStringList = identifierOrStringList);
        }

        public SyntaxToken OnKeyword => ChildTokens().FirstOrMissing(SyntaxTokenType.OnKeyword);

        [CanBeNull]
        public IdentifierOrStringListSyntax IdentifierOrStringList => _identifierOrStringList;
    }
}