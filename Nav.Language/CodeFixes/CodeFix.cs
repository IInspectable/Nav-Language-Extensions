#region Using Directives

using System;
using System.Collections.Generic;

using Pharmatechnik.Nav.Language.Text;

#endregion

namespace Pharmatechnik.Nav.Language.CodeFixes {

    public abstract class CodeFix {
        
        protected CodeFix(CodeFixContext context) {
            Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public CodeFixContext Context { get; }
        public abstract TextExtent? ApplicableTo { get; }
        public abstract CodeFixPrio Prio { get; }
        public CodeGenerationUnit CodeGenerationUnit => Context.CodeGenerationUnit;
        public CodeGenerationUnitSyntax Syntax       => CodeGenerationUnit.Syntax;
        public SyntaxTree SyntaxTree                 => Syntax.SyntaxTree;

        public abstract string Name { get; }
        public abstract CodeFixImpact Impact { get; }
        
        protected static IEnumerable<TextChange> GetRemoveChanges(TextExtent extent) {
            if (extent.IsMissing) {
                yield break;
            }
            yield return TextChange.NewRemove(extent);
        }

        protected static IEnumerable<TextChange> GetInsertChanges(int position, string newText) {
            if (newText == null) {
                yield break;
            }
            yield return TextChange.NewInsert(position, newText);
        }

        protected IEnumerable<TextChange> GetRemoveSyntaxNodeChanges(SyntaxNode syntaxNode) {
            return SyntaxTree.GetRemoveSyntaxNodeChanges(syntaxNode, Context.EditorSettings);            
        }
        
        protected IEnumerable<TextChange> GetRenameSourceChanges(ITransition transition, string newSourceName) {
            return SyntaxTree.GetRenameSourceChanges(transition, newSourceName, Context.EditorSettings);
        }

        protected IEnumerable<TextChange> GetRenameSourceChanges(IExitTransition transition, string newSourceName) {
            return SyntaxTree.GetRenameSourceChanges(transition, newSourceName, Context.EditorSettings);
        }

        protected static IEnumerable<TextChange> GetRenameTargetChanges(IEdge transition, string newSourceName) {
            return GetRenameSymbolChanges(transition.TargetReference, newSourceName);
        }

        protected static IEnumerable<TextChange> GetRenameSymbolChanges(ISymbol symbol, string newName) {
            if (symbol == null || symbol.Name == newName) {
                yield break;
            }
            yield return TextChange.NewReplace(symbol.Location.Extent, newName);
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