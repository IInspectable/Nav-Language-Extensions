#region Using Directives

using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

using JetBrains.Annotations;
using Pharmatechnik.Nav.Language.Internal;

#endregion

namespace Pharmatechnik.Nav.Language {

    [Serializable]
    [DebuggerDisplay("{ToDebuggerDisplayString(), nq}")]
    public abstract partial class SyntaxNode: IExtent {

        readonly TextExtent _extent;

        List<SyntaxNode> _childNodes;
        SyntaxTree _syntaxTree;
        SyntaxNode _parent;

        internal SyntaxNode(TextExtent extent) {
            _extent     = extent;
        }

        internal void FinalConstruct(SyntaxTree syntaxTree, SyntaxNode parent) {

            EnsureConstructionMode();

            _syntaxTree = syntaxTree;
            _parent     = parent;

            if (_childNodes == null) {
                return;
            }

            foreach (var child in _childNodes) {
                child.FinalConstruct(syntaxTree, this);
            }
        }

        public int Start { get { return Extent.Start; } }
        public int End { get { return Extent.End; } }
        public int Length { get { return Extent.Length; } }

        public TextExtent Extent {
            get { return _extent; }
        }

        [CanBeNull]
        public SyntaxNode Parent {
            get {
                EnsureConstructed();
                return _parent;
            }
        }       

        public Location GetLocation() {
            EnsureConstructed();
            return SyntaxTree.GetLocation(Extent);
        }

        [NotNull]
        public IEnumerable<SyntaxToken> ChildTokens() {
            return SyntaxTree.Tokens[Extent].Where(token => token.Parent== this);
        }

        static readonly IReadOnlyList<SyntaxNode> EmptyNodeList = new List<SyntaxNode>();

        [NotNull]
        public IReadOnlyList<SyntaxNode> ChildNodes() {
            EnsureConstructed();
            return _childNodes?? EmptyNodeList;
        }

        [NotNull]
        public IEnumerable<SyntaxNode> DescendantNodes() {
            return DescendantNodes<SyntaxNode>();
        }

        [NotNull]
        public IEnumerable<SyntaxNode> DescendantNodesAndSelf() {
            return DescendantNodesAndSelf<SyntaxNode>();
        }

        [NotNull]
        public IEnumerable<T> DescendantNodes<T>() where T : SyntaxNode {
            return DescendantNodesAndSelfImpl<T>(includeSelf: false);
        }

        [NotNull]
        public IEnumerable<T> DescendantNodesAndSelf<T>() where T : SyntaxNode {
            return DescendantNodesAndSelfImpl<T>(includeSelf: true);
        }

        [NotNull]
        IEnumerable<T> DescendantNodesAndSelfImpl<T>(bool includeSelf) where T : SyntaxNode {
            EnsureConstructed();
            if (includeSelf && this is T) {
                yield return (T)this;

                if(typeof(T) == GetType() && PromiseNoDescendantNodeOfSameType) {
                    yield break;
                }
            }
            foreach (var node in ChildNodes().SelectMany(child => child.DescendantNodesAndSelf<T>())) {
                yield return node;
            }
        }

        /// <summary>
        /// Für Knoten, die sehr weit "oben" liegen, kann die Implementierung von DescendantNodes&lt;T&gt;
        /// massiv beschleunigt werden, wenn sichergestellt werden kann, dass ein Knoten keine untergeordnenten 
        /// Knoten vom selben Typ haben kann, und deshalb die Suche in den Kindknoten vorzeitig abgebrochen
        /// werden kann.
        /// Eigentlich müsste diese Eigenschaft systematisch überschrieben werden. Bisweilen werden hiermit nur
        /// die Hotspots optimiert.
        /// </summary>
        protected virtual bool PromiseNoDescendantNodeOfSameType {
            get { return false; }
        }

        /// <summary>
        /// Gets a list of ancestor nodes
        /// </summary>
        public IEnumerable<SyntaxNode> Ancestors() {
            return Parent?.AncestorsAndSelf() ?? Enumerable.Empty<SyntaxNode>();
        }

        /// <summary>
        /// Gets a list of ancestor nodes (including this node) 
        /// </summary>
        public IEnumerable<SyntaxNode> AncestorsAndSelf() {
            for (var node = this; node != null; node = node.Parent) {
                yield return node;
            }
        }
        
        public SyntaxNode FindNode(int position) {
            var token = SyntaxTree.Tokens.FindAtPosition(position);
            if (token.IsMissing) {
                return null;
            }
            if (Extent.Contains(token.Extent)) {
                return token.Parent;
            }
            return null;
        }

        public TextExtent GetFullExtent() {
            return TextExtent.FromBounds(GetLeadingTriviaExtent().Start, GetTrailingTriviaExtent().End);
        }

        // TODO Implement GetLeadingTriviaExtent
        public TextExtent GetLeadingTriviaExtent() {
            
            if (Start == 0) {
                return TextExtent.Empty;
            }
            
            var index     = SyntaxTree.Tokens.FindIndexAtPosition(Start - 1);
            var nextToken = SyntaxTree.Tokens[index+1];
            var start     = 0;
            while (index > 0) {

                var token=SyntaxTree.Tokens[index];
                if (!SyntaxFacts.IsTrivia(token.Classification)) {
                    start = nextToken.Type==SyntaxTokenType.NewLine? nextToken.End: token.End;
                    break;
                }
     
                nextToken = token;
                index--;
            }
            
            return TextExtent.FromBounds(start, Start);
        }

        // TODO Implement GetTrailingTriviaExtent
        public TextExtent GetTrailingTriviaExtent() {

            var endLine = SyntaxTree.GetTextLineExtent(End);

            var start = End;
            var end = endLine.Extent.End;

            var trailingExtent = TextExtent.FromBounds(End, endLine.Extent.End);

            var endToken = SyntaxTree.Tokens[trailingExtent]
                                     .SkipWhile(token => SyntaxFacts.IsTrivia(token.Classification))
                                     .FirstOrDefault();

            if (!endToken.IsMissing) {
                end = endToken.Type == SyntaxTokenType.NewLine ? endToken.End : endToken.Start;
            }

            return TextExtent.FromBounds(start, end);
        }

        [NotNull]
        public SyntaxTree SyntaxTree {
            get {
                EnsureConstructed();
                return _syntaxTree;
            }
        }
        
        protected void AddChildNode(SyntaxNode syntaxNode) {
            EnsureConstructionMode();
            EnsureChildNodes();
            if (syntaxNode != null) {                
                _childNodes.Add(syntaxNode);
            }
        }
        
        protected void AddChildNodes(IEnumerable<SyntaxNode> syntaxNodes) {
            EnsureConstructionMode();
            EnsureChildNodes();
            foreach (var node in syntaxNodes) {
                AddChildNode(node);
            }
        }

        void EnsureConstructed() {
            if (_syntaxTree == null) {
                throw new InvalidOperationException();
            }
        }

        void EnsureChildNodes() {
            if (_childNodes == null) {
                EnsureConstructionMode();
                _childNodes = new List<SyntaxNode>();
            }
        }

        void EnsureConstructionMode() {
            if (_syntaxTree != null) {
                throw new InvalidOperationException();
            }
        }
        
        public override string ToString() {
            return SyntaxTree.SourceText.Substring(Start, Length);
        }

        public string ToDebuggerDisplayString() {
            return $"{Extent} {GetType().Name}";
        }
    }
}