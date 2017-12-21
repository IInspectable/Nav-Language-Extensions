#region Using Directives

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using JetBrains.Annotations;
using Pharmatechnik.Nav.Language.Internal;

#endregion

namespace Pharmatechnik.Nav.Language {

    [Serializable]
    public sealed class SyntaxTokenList: IReadOnlyList<SyntaxToken> {

        static readonly IReadOnlyList<SyntaxToken> EmptyTokens= new List<SyntaxToken>(Enumerable.Empty<SyntaxToken>()).AsReadOnly();
        readonly IReadOnlyList<SyntaxToken> _tokens;

        public SyntaxTokenList(List<SyntaxToken> tokens): this(tokens, attachSorted: false) {
        }

        SyntaxTokenList(IReadOnlyList<SyntaxToken> tokens, bool attachSorted) {

            if(attachSorted || tokens==null || tokens.Count==0) {
                // Tokens sind bereits sortiert oder es gibt keine Tokens
                _tokens = tokens ?? EmptyTokens;
            } else {
                var tokenList = new List<SyntaxToken>(tokens);
                tokenList.Sort(SyntaxTokenComparer.Default);
                _tokens = tokenList;
            }            
        }

        internal static SyntaxTokenList AttachSortedTokens(IReadOnlyList<SyntaxToken> tokens) {
            return new SyntaxTokenList(tokens, attachSorted: true);
        }

        public static readonly SyntaxTokenList Empty = new SyntaxTokenList(null);

        public IEnumerator<SyntaxToken> GetEnumerator() {
            return _tokens.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public int Count => _tokens.Count;

        public SyntaxToken this[int index] => _tokens[index];

        [NotNull]
        public IEnumerable<SyntaxToken> this[TextExtent extent, bool includeOverlapping = false] => _tokens.GetElements(extent, includeOverlapping);

        public SyntaxToken FindAtPosition(int position) {
            if(position < 0) {
                return SyntaxToken.Missing;
            }
            return _tokens.FindElementAtPosition(position, defaultIfNotFound: true);
        }

        internal SyntaxToken NextOrPrevious(SyntaxNode node, SyntaxToken currentToken, SyntaxTokenType type, bool nextToken) {
            SyntaxToken token = currentToken;
            while (!(token = NextOrPrevious(node, token, nextToken)).IsMissing) {
                if (token.Type == type) {
                    return token;
                }
            }
            return SyntaxToken.Missing;
        }

        internal SyntaxToken NextOrPrevious(SyntaxNode node, SyntaxToken currentToken, SyntaxTokenClassification tokenClassification, bool nextToken) {
            SyntaxToken token = currentToken;
            while (!(token = NextOrPrevious(node, token, nextToken)).IsMissing) {
                if (token.Classification == tokenClassification) {
                    return token;
                }
            }
            return SyntaxToken.Missing;
        }

        internal SyntaxToken NextOrPrevious(SyntaxNode node, SyntaxToken currentToken, bool nextToken) {
            return _tokens.NextOrPreviousElement(node, currentToken, nextToken, SyntaxToken.Missing);
        }
    }
}
