using System;
using JetBrains.Annotations;

namespace Pharmatechnik.Nav.Language {

    [Serializable]
    public abstract class TriggerSyntax : SyntaxNode {
        protected TriggerSyntax(TextExtent extent) : base(extent) {}       
    }

    [Serializable]
    [SampleSyntax("spontaneous")]
    public partial class SpontaneousTriggerSyntax : TriggerSyntax {
        internal SpontaneousTriggerSyntax(TextExtent extent) : base(extent) {}
        
        public SyntaxToken SpontaneousKeyword {
            get { return ChildTokens().FirstOrMissing(SyntaxTokenType.SpontaneousKeyword); }
        }

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

        public SyntaxToken OnKeyword {
            get { return ChildTokens().FirstOrMissing(SyntaxTokenType.OnKeyword); }
        }

        [CanBeNull]
        public IdentifierOrStringListSyntax IdentifierOrStringList {
            get { return _identifierOrStringList; }
        }
    }
}