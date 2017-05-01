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
        public abstract bool CanApplyFix();

        [CanBeNull]
        protected static TextChange? NewReplace(ISymbol symbol, string newName) {
            if (symbol == null || symbol.Name == newName) {
                return null;
            }
            return new TextChange(symbol.Location.Extent, newName);
        }

        [CanBeNull]
        protected static TextChange? NewInsert(int position, string newText) {
            return new TextChange(TextExtent.FromBounds(position, position), newText);
        }

        protected string ComposeEdge(IEdge templateEdge, string sourceName, string edgeKeyword, string targetName) {

            string indent = new string(' ', EditorSettings.TabSize);
            if (templateEdge.Source != null) {
                var templateEdgeLine = SyntaxTree.GetTextLineExtent(templateEdge.Source.Start);
                indent = GetIndent(templateEdgeLine);
            }

            var exitTransition = $"{indent}{sourceName}{WhiteSpaceBetweenSourceAndEdgeMode(templateEdge, sourceName)}{edgeKeyword}{WhiteSpaceBetweenEdgeModeAndTarget(templateEdge)}{targetName}{SyntaxFacts.Semicolon}";
            return exitTransition;
        }

        protected string GetIndent(TextLineExtent lineExtent) {

            var line = SyntaxTree.SourceText.Substring(lineExtent.Extent.Start, lineExtent.Extent.Length);

            var startColumn = line.GetSignificantColumn(EditorSettings.TabSize);

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

        protected string WhiteSpaceBetweenSourceAndEdgeMode(IEdge sampleEdge, string newSourceName) {

            if (sampleEdge.Source == null || sampleEdge.EdgeMode == null) {
                return " ";
            }

            var oldOffset = ColumnsBetweenLocations(sampleEdge.Source.Location, sampleEdge.EdgeMode.Location);

            var oldLength = sampleEdge.Source.Location.Length;
            var newLength = newSourceName.Length;
            var offset = Math.Max(1, oldOffset + oldLength - newLength);

            return new String(' ', offset);
        }

        protected string WhiteSpaceBetweenEdgeModeAndTarget(IEdge sampleEdge) {

            if (sampleEdge.EdgeMode == null || sampleEdge.Target == null) {
                return " ";
            }
            var offset = ColumnsBetweenLocations(sampleEdge.EdgeMode.Location, sampleEdge.Target.Location);
            return new String(' ', offset);
        }

        protected int ColumnsBetweenLocations(Location location1, Location location2) {

            if (location1 == null || location2 == null) {
                return 0;
            }

            int spaceCount = 1;
            if (location1.EndLine != location2.StartLine) {
                // Locations in unterschiedliche Zeilen
                var column = SyntaxTree.GetStartColumn(location2, EditorSettings.TabSize);
                spaceCount = column;
            }
            else {
                // Locations in selber Zeile
                var startColumn = SyntaxTree.GetEndColumn(location1, EditorSettings.TabSize);
                var endColumn = SyntaxTree.GetStartColumn(location2, EditorSettings.TabSize);

                spaceCount = Math.Max(1, endColumn - startColumn);
            }

            return Math.Max(1, spaceCount);
        }

        protected int ColumnsBetweenKeywordAndIdentifier(INodeSymbol sampleNode, string newKeyword = null) {

            var locations = KeywordAndIdentifierFinder.Find(sampleNode.Syntax);
            if (locations == null) {
                return 1;
            }

            var oldOffset = ColumnsBetweenLocations(locations.Item1, locations.Item2);

            var oldLength  = locations.Item1.Length;
            var newLength  = newKeyword?.Length ?? oldLength;
            var spaceCount = Math.Max(1, oldOffset + oldLength - newLength);

            return spaceCount;
        }

        sealed class KeywordAndIdentifierFinder : SyntaxNodeVisitor<Tuple<Location, Location>> {

            public static Tuple<Location, Location> Find(NodeDeclarationSyntax nodeDeclaration) {

                var finder = new KeywordAndIdentifierFinder();
                return finder.Visit(nodeDeclaration);
            }

            public override Tuple<Location, Location> VisitChoiceNodeDeclaration(ChoiceNodeDeclarationSyntax choiceNodeDeclarationSyntax) {
                return SafeCreateTuple(choiceNodeDeclarationSyntax.ChoiceKeyword, choiceNodeDeclarationSyntax.Identifier);
            }

            public override Tuple<Location, Location> VisitEndNodeDeclaration(EndNodeDeclarationSyntax endNodeDeclarationSyntax) {
                // End hat keinen Identifier
                return DefaultVisit(endNodeDeclarationSyntax);
            }

            public override Tuple<Location, Location> VisitExitNodeDeclaration(ExitNodeDeclarationSyntax exitNodeDeclarationSyntax) {
                return SafeCreateTuple(exitNodeDeclarationSyntax.ExitKeyword, exitNodeDeclarationSyntax.Identifier);
            }

            public override Tuple<Location, Location> VisitInitNodeDeclaration(InitNodeDeclarationSyntax initNodeDeclarationSyntax) {
                return SafeCreateTuple(initNodeDeclarationSyntax.InitKeyword, initNodeDeclarationSyntax.Identifier);
            }

            public override Tuple<Location, Location> VisitDialogNodeDeclaration(DialogNodeDeclarationSyntax dialogNodeDeclarationSyntax) {
                return SafeCreateTuple(dialogNodeDeclarationSyntax.DialogKeyword, dialogNodeDeclarationSyntax.Identifier);
            }

            public override Tuple<Location, Location> VisitTaskNodeDeclaration(TaskNodeDeclarationSyntax taskNodeDeclarationSyntax) {
                return SafeCreateTuple(taskNodeDeclarationSyntax.TaskKeyword, taskNodeDeclarationSyntax.Identifier);
            }

            public override Tuple<Location, Location> VisitViewNodeDeclaration(ViewNodeDeclarationSyntax viewNodeDeclarationSyntax) {
                return SafeCreateTuple(viewNodeDeclarationSyntax.ViewKeyword, viewNodeDeclarationSyntax.Identifier);
            }

            Tuple<Location, Location> SafeCreateTuple(SyntaxToken token1, SyntaxToken token2) {
                if (token1.IsMissing || token2.IsMissing) {
                    return null;
                }
                return new Tuple<Location, Location>(token1.GetLocation(), token2.GetLocation());
            }
        }
    }
}