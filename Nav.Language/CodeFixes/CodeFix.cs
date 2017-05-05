#region Using Directives

using System;

using JetBrains.Annotations;
using Pharmatechnik.Nav.Language.Text;

#endregion

namespace Pharmatechnik.Nav.Language.CodeFixes {

    public abstract class CodeFix {
        
        protected CodeFix(CodeFixContext context) {
            Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public CodeFixContext Context { get; }
        public abstract TextExtent? ApplicableTo { get; }
        public CodeGenerationUnit CodeGenerationUnit => Context.CodeGenerationUnit;
        public CodeGenerationUnitSyntax Syntax       => CodeGenerationUnit.Syntax;
        public SyntaxTree SyntaxTree                 => Syntax.SyntaxTree;

        public abstract string Name { get; }
        public abstract CodeFixImpact Impact { get; }

        [CanBeNull]
        protected static TextChange? TryRemove(TextExtent extent) {
            if (extent.IsMissing) {
                return null;
            }
            return new TextChange(extent, String.Empty);
        }

        [CanBeNull]
        protected static TextChange? TryRename(ISymbol symbol, string newName) {
            if (symbol == null || symbol.Name == newName) {
                return null;
            }
            return new TextChange(symbol.Location.Extent, newName);
        }

        [CanBeNull]
        protected static TextChange? TryInsert(int position, string newText) {
            if (newText == null) {
                return null;
            }
            return new TextChange(TextExtent.FromBounds(position, position), newText);
        }

        [CanBeNull]
        protected TextChange? TryRenameSource(ITransition transition, string newSourceName) {
            return SyntaxTree.TryRenameSource(transition, newSourceName, Context.EditorSettings);
        }

        [CanBeNull]
        protected TextChange? TryRenameSource(IExitTransition transition, string newSourceName) {
            return SyntaxTree.TryRenameSource(transition, newSourceName, Context.EditorSettings);
        }

        [CanBeNull]
        protected static TextChange? TryRenameTarget(IEdge transition, string newSourceName) {
            return TryRename(transition.Target, newSourceName);
        }

        protected string ComposeEdge(IEdge templateEdge, string sourceName, string edgeKeyword, string targetName) {
            return SyntaxTree.ComposeEdge(templateEdge, sourceName, edgeKeyword, targetName, Context.EditorSettings);
        }

        protected string WhiteSpaceBetweenSourceAndEdgeMode(IEdge edge, string newSourceName) {
            return SyntaxTree.WhiteSpaceBetweenSourceAndEdgeMode(edge, newSourceName, Context.EditorSettings);
        }

        protected string WhiteSpaceBetweenEdgeModeAndTarget(IEdge edge) {
            return SyntaxTree.WhiteSpaceBetweenEdgeModeAndTarget(edge, Context.EditorSettings);
        }

        protected int ColumnsBetweenKeywordAndIdentifier(INodeSymbol node, string newKeyword = null) {
            return SyntaxTree.ColumnsBetweenKeywordAndIdentifier(node, newKeyword, Context.EditorSettings);
        }

        protected string GetLineIndent(TextLineExtent lineExtent) {
            return SyntaxTree.GetLineIndent(lineExtent, Context.EditorSettings);
        }        
    }
}