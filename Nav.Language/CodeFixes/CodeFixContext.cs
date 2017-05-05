#region Using Directives

using System;
using System.Linq;
using System.Collections.Generic;

#endregion

namespace Pharmatechnik.Nav.Language.CodeFixes {
    public sealed class CodeFixContext {

        public CodeFixContext(TextExtent extent, CodeGenerationUnit codeGenerationUnit, EditorSettings editorSettings) {
            
            CodeGenerationUnit = codeGenerationUnit ?? throw new ArgumentNullException(nameof(codeGenerationUnit));
            EditorSettings     = editorSettings     ?? throw new ArgumentNullException(nameof(editorSettings));
            Range              = extent;

            // TODO Verify, Unit Tests
        }

        public TextExtent Range { get; }
        public CodeGenerationUnit CodeGenerationUnit { get; }
        public EditorSettings EditorSettings { get; }
        
        public IEnumerable<ISymbol> FindSymbols(bool includeOverlapping = false) {
            return CodeGenerationUnit.Symbols[Range];
        }

        public IEnumerable<T> FindSymbols<T>(bool includeOverlapping = false) where T : ISymbol {
            return FindSymbols().OfType<T>();
        }

        public IEnumerable<T> FindNodes<T>(bool includeOverlapping = false) where T : SyntaxNode {
            var candidates = FindTokens().Select(t => t.Parent).OfType<T>();
            if (!includeOverlapping) {
                return candidates.Where(node => Range.IntersectsWith(node.Extent));
            }
            return candidates;
        }

        public IEnumerable<SyntaxToken> FindTokens() {
            return CodeGenerationUnit.Syntax.SyntaxTree.Tokens[Range];
        }

        public bool ContainsNodes<T>() where T : SyntaxNode {
            return FindNodes<T>().Any();
        }        
    }
}