#region Using Directives

using System;
using System.Linq;
using System.Collections.Generic;
using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Nav.Language.CodeFixes {
    public sealed class CodeFixContext {

        public CodeFixContext(int position, CodeGenerationUnit codeGenerationUnit, EditorSettings editorSettings) {
            // TODO Verify, Unit Tests

            Position           = position;
            CodeGenerationUnit = codeGenerationUnit ?? throw new ArgumentNullException(nameof(codeGenerationUnit));
            EditorSettings     = editorSettings     ?? throw new ArgumentNullException(nameof(editorSettings));
            LineExtent         = codeGenerationUnit.Syntax.SyntaxTree.GetTextLineExtent(position);                       
        }

        public int Position { get; }
        public TextLineExtent LineExtent { get; }
        public CodeGenerationUnit CodeGenerationUnit { get; }
        public EditorSettings EditorSettings { get; }

        [CanBeNull]
        public ISymbol TryFindSymbolAtPosition() {
            var symbol = CodeGenerationUnit.Symbols.FindAtPosition(Position);
            if (symbol == null && Position > LineExtent.Extent.Start) {
                symbol = CodeGenerationUnit.Symbols.FindAtPosition(Position - 1);
            }
            return symbol;
        }

        [CanBeNull]
        public SyntaxNode TryFindNodeAtPosition() {
            var token = CodeGenerationUnit.Syntax.SyntaxTree.Tokens.FindAtPosition(Position);
            if (token.Classification == SyntaxTokenClassification.Whitespace && Position > LineExtent.Extent.Start) {
                token = CodeGenerationUnit.Syntax.SyntaxTree.Tokens.FindAtPosition(Position - 1);
            }
            return token.Parent;
        }

        public IEnumerable<T> FindNodes<T>(bool includeOverlapping = false) where T : SyntaxNode {
            var candidates = FindTokens().Select(t => t.Parent).OfType<T>();
            if (!includeOverlapping) {
                return candidates.Where(node => LineExtent.Extent.IntersectsWith(node.Extent));
            }
            return candidates;
        }

        public IEnumerable<SyntaxToken> FindTokens() {
            return CodeGenerationUnit.Syntax.SyntaxTree.Tokens[LineExtent.Extent];
        }

        public bool ContainsNodes<T>() where T : SyntaxNode {
            return FindNodes<T>().Any();
        }        
    }
}