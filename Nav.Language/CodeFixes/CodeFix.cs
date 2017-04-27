#region Using Directives

using System;
using System.Collections.Generic;
using Pharmatechnik.Nav.Language.Text;

#endregion

namespace Pharmatechnik.Nav.Language.CodeFixes {
    public abstract class CodeFix {

        protected CodeFix(CodeGenerationUnit codeGenerationUnit) {
            CodeGenerationUnit = codeGenerationUnit ?? throw new ArgumentNullException(nameof(codeGenerationUnit));
        }

        public CodeGenerationUnit CodeGenerationUnit { get; }
        public CodeGenerationUnitSyntax Syntax => CodeGenerationUnit.Syntax;
        public SyntaxTree SyntaxTree => Syntax.SyntaxTree;

        public abstract bool CanApplyFix();

        protected static TextChange? NewReplace(ISymbol symbol, string newName) {
            if (symbol == null || symbol.Name == newName) {
                return null;
            }
            return new TextChange(symbol.Location.Extent, newName);
        }

        protected static TextChange? NewInsert(int position, string newText) {
            return new TextChange(TextExtent.FromBounds(position, position), newText);
        }

        protected string GetSignificantColumn(TextLineExtent lineExtent, int tabSize) {

            var line = SyntaxTree.SourceText.Substring(lineExtent.Extent.Start, lineExtent.Extent.Length);

            var startColumn = line.GetSignificantColumn(tabSize);

            return new String(' ', startColumn);
        }

        protected HashSet<string> GetDeclaredNodeNames(ITaskDefinitionSymbol taskDefinitionSymbol) {

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