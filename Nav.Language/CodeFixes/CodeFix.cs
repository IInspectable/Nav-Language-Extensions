#region Using Directives

using System;
using System.Collections.Generic;

using JetBrains.Annotations;
using Pharmatechnik.Nav.Language.Text;

#endregion

namespace Pharmatechnik.Nav.Language.CodeFixes {

    public abstract class CodeFix {
      
        protected CodeFix(EditorSettings editorSettings, CodeGenerationUnit codeGenerationUnit) {
            EditorSettings     = editorSettings     ?? throw new ArgumentNullException(nameof(editorSettings));
            CodeGenerationUnit = codeGenerationUnit ?? throw new ArgumentNullException(nameof(codeGenerationUnit));
        }
        
        public EditorSettings EditorSettings { get; }
        public CodeGenerationUnit CodeGenerationUnit { get; }
        public CodeGenerationUnitSyntax Syntax => CodeGenerationUnit.Syntax;
        public SyntaxTree SyntaxTree => Syntax.SyntaxTree;

        public abstract string Name { get; }
        public abstract CodeFixImpact Impact { get; }
        public abstract bool CanApplyFix();

        protected string ValidateNewNodeName(string nodeName, ITaskDefinitionSymbol taskDefinitionSymbol) {

            nodeName = nodeName?.Trim();

            if (!SyntaxFacts.IsValidIdentifier(nodeName) || SyntaxFacts.IsKeyword(nodeName)) {
                return DiagnosticDescriptors.Semantic.Nav2000IdentifierExpected.MessageFormat;
            }

            var declaredNodeNames = GetDeclaredNodeNames(taskDefinitionSymbol);
            if (declaredNodeNames.Contains(nodeName)) {
                return String.Format(DiagnosticDescriptors.Semantic.Nav0022NodeWithName0AlreadyDeclared.MessageFormat, nodeName);
            }

            return null;
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
            if(newText == null) {
                return null;
            }
            return new TextChange(TextExtent.FromBounds(position, position), newText);
        }

        [CanBeNull]
        protected TextChange? TryRenameSource(ITransition transition, string newSourceName) {
            return SyntaxTree.TryRenameSource(transition, newSourceName, EditorSettings);       
        }

        [CanBeNull]
        protected TextChange? TryRenameSource(IExitTransition transition, string newSourceName) {
            return SyntaxTree.TryRenameSource(transition, newSourceName, EditorSettings);           
        }

        [CanBeNull]
        protected static TextChange? TryRenameTarget(IEdge transition, string newSourceName) {
            return TryRename(transition.Target, newSourceName);
        }

        protected string ComposeEdge(IEdge templateEdge, string sourceName, string edgeKeyword, string targetName) {
            return SyntaxTree.ComposeEdge(templateEdge, sourceName, edgeKeyword, targetName, EditorSettings);
        }
        
        protected string WhiteSpaceBetweenSourceAndEdgeMode(IEdge edge, string newSourceName) {
            return SyntaxTree.WhiteSpaceBetweenSourceAndEdgeMode(edge, newSourceName, EditorSettings);          
        }

        protected string WhiteSpaceBetweenEdgeModeAndTarget(IEdge edge) {
            return SyntaxTree.WhiteSpaceBetweenEdgeModeAndTarget(edge, EditorSettings);
        }
        
        protected int ColumnsBetweenKeywordAndIdentifier(INodeSymbol node, string newKeyword = null) {
            return SyntaxTree.ColumnsBetweenKeywordAndIdentifier(node, newKeyword, EditorSettings);           
        }

        protected string GetLineIndent(TextLineExtent lineExtent) {
            return SyntaxTree.GetLineIndent(lineExtent, EditorSettings);
        }

        HashSet<string> GetDeclaredNodeNames(ITaskDefinitionSymbol taskDefinitionSymbol) {

            var declaredNodeNames = new HashSet<string>();
            if (taskDefinitionSymbol == null) {
                return declaredNodeNames;
            }

            foreach (var node in taskDefinitionSymbol.NodeDeclarations) {
                var nodeName = node.Name;
                if (!String.IsNullOrEmpty(nodeName)) {
                    declaredNodeNames.Add(nodeName);
                }
            }
            return declaredNodeNames;
        }
    }
}