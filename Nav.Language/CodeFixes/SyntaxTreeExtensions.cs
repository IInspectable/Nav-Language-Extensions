#region Using Directives

using System;
using System.Linq;
using System.Collections.Generic;

using JetBrains.Annotations;

using Pharmatechnik.Nav.Language.Text;

#endregion

namespace Pharmatechnik.Nav.Language.CodeFixes {

    static class SyntaxTreeExtensions {

        [NotNull]
        public static IEnumerable<TextChange> GetRemoveSyntaxNodeChanges(this SyntaxTree syntaxTree, SyntaxNode syntaxNode, EditorSettings editorSettings) {

            var fullExtent = syntaxNode.GetFullExtent(onlyWhiteSpace: true);
            yield return new TextChange(fullExtent, String.Empty);

            var lineExtent = syntaxTree.GetTextLineExtentAtPosition(fullExtent.End - 1).Extent;
            // Prinzipiell enthalten die TrailingTrivia auch das NL Token. Wenn wir aber nicht die einzige Syntax in der Zeile sind,
            // soll das NL erhalten bleiben. Deswegen schieben wir das durch den fullExtent gelöschte NL hier wieder ein.
            if (fullExtent.Start > lineExtent.Start && fullExtent.End == lineExtent.End) {
                yield return new TextChange(TextExtent.FromBounds(lineExtent.End, lineExtent.End), editorSettings.NewLine);
            }
        }

        [NotNull]
        public static IEnumerable<TextChange> GetRenameSourceChanges(this SyntaxTree syntaxTree, ITransition transition, string newSourceName, EditorSettings editorSettings) {

            if (transition?.Source == null) {
                yield break;
            }

            var replaceText = newSourceName;
            var replaceLocation = transition.Source.Location;

            var replaceExtent = replaceLocation.Extent;
            if (transition.EdgeMode != null && transition.Source.Location.EndLine == transition.EdgeMode.Location.StartLine) {
                // Find the First non-Whitespace Token after Source Edge
                var firstNoneWhitespaceToken = syntaxTree.FirstNoneWhitespaceToken(TextExtent.FromBounds(replaceLocation.End, transition.EdgeMode.End));
                if (!firstNoneWhitespaceToken.IsMissing) {
                    var availableSpace = replaceLocation.Length + syntaxTree.ColumnsBetweenLocations(replaceLocation, firstNoneWhitespaceToken.GetLocation(), editorSettings);

                    replaceExtent = TextExtent.FromBounds(replaceLocation.Start, firstNoneWhitespaceToken.Start);

                    var spaces = Math.Max(1, availableSpace - newSourceName.Length);

                    replaceText = newSourceName + new string(' ', spaces);
                }
            }

            yield return new TextChange(replaceExtent, replaceText);
        }

        [NotNull]
        public static IEnumerable<TextChange> GetRenameSourceChanges(this SyntaxTree syntaxTree, IExitTransition transition, string newSourceName, EditorSettings editorSettings) {

            if (transition?.Source == null || transition.ConnectionPoint == null) {
                yield break;
            }

            var replaceText = $"{newSourceName}{SyntaxFacts.Colon}{transition.ConnectionPoint.Name}";
            var replaceLocation = new Location(
                extent: TextExtent.FromBounds(transition.Source.Start, transition.ConnectionPoint.End),
                linePositionExtent: new LinePositionExtent(
                    start: transition.Source.Location.StartLinePosition,
                    end: transition.ConnectionPoint.Location.EndLinePosition),
                filePath: transition.ConnectionPoint.Location.FilePath);

            var replaceExtent = replaceLocation.Extent;
            if (transition.EdgeMode != null && transition.Source.Location.EndLine == transition.EdgeMode.Location.StartLine) {
                // Find the First non-Whitespace Token after Source Edge
                var firstNoneWhitespaceToken = syntaxTree.FirstNoneWhitespaceToken(TextExtent.FromBounds(replaceLocation.End, transition.EdgeMode.End));
                if (!firstNoneWhitespaceToken.IsMissing) {

                    var availableSpace = replaceLocation.Length + syntaxTree.ColumnsBetweenLocations(replaceLocation, firstNoneWhitespaceToken.GetLocation(), editorSettings);

                    replaceExtent = TextExtent.FromBounds(replaceLocation.Start, firstNoneWhitespaceToken.Start);

                    var spaces = Math.Max(1, availableSpace - replaceText.Length);

                    replaceText = replaceText + new string(' ', spaces);
                }
            }

            yield return new TextChange(replaceExtent, replaceText);
        }

        public static string ComposeEdge(this SyntaxTree syntaxTree, IEdge templateEdge, string sourceName, string edgeKeyword, string targetName, EditorSettings editorSettings) {

            string indent = new string(' ', editorSettings.TabSize);
            if (templateEdge.Source != null) {
                var templateEdgeLine = syntaxTree.GetTextLineExtentAtPosition(templateEdge.Source.Start);
                indent = syntaxTree.GetLineIndent(templateEdgeLine, editorSettings);
            }

            var whiteSpaceBetweenSourceAndEdgeMode = syntaxTree.WhiteSpaceBetweenSourceAndEdgeMode(templateEdge, sourceName, editorSettings);
            var whiteSpaceBetweenEdgeModeAndTarget = syntaxTree.WhiteSpaceBetweenEdgeModeAndTarget(templateEdge, editorSettings);

            var exitTransition = $"{indent}{sourceName}{whiteSpaceBetweenSourceAndEdgeMode}{edgeKeyword}{whiteSpaceBetweenEdgeModeAndTarget}{targetName}{SyntaxFacts.Semicolon}";
            return exitTransition;
        }

        public static string WhiteSpaceBetweenSourceAndEdgeMode(this SyntaxTree syntaxTree, IEdge edge, string newSourceName, EditorSettings editorSettings) {

            if (edge.Source == null || edge.EdgeMode == null) {
                return " ";
            }

            var oldOffset = syntaxTree.ColumnsBetweenLocations(edge.Source.Location, edge.EdgeMode.Location, editorSettings);

            var oldLength = edge.Source.Location.Length;
            var newLength = newSourceName.Length;
            var offset = Math.Max(1, oldOffset + oldLength - newLength);

            return new String(' ', offset);
        }

        public static string WhiteSpaceBetweenEdgeModeAndTarget(this SyntaxTree syntaxTree, IEdge edge, EditorSettings editorSettings) {

            if (edge.EdgeMode == null || edge.Target == null) {
                return " ";
            }
            var offset = syntaxTree.ColumnsBetweenLocations(edge.EdgeMode.Location, edge.Target.Location, editorSettings);
            return new String(' ', offset);
        }

        public static int ColumnsBetweenLocations(this SyntaxTree syntaxTree, Location location1, Location location2, EditorSettings editorSettings) {

            if (location1 == null || location2 == null) {
                return 0;
            }
   
            int spaceCount = 1;
            if (location1.EndLine != location2.StartLine) {
                // Locations in unterschiedliche Zeilen
                var column = syntaxTree.GetStartColumn(location2, editorSettings);
                spaceCount = column;
            }
            else {
                // Locations in selber Zeile
                var startColumn = syntaxTree.GetEndColumn(location1, editorSettings);
                var endColumn = syntaxTree.GetStartColumn(location2, editorSettings);

                spaceCount = Math.Max(1, endColumn - startColumn);
            }

            return Math.Max(1, spaceCount);
        }

        public static int GetEndColumn(this SyntaxTree syntaxTree, Location location, EditorSettings editorSettings) {
            var endLineExtent = syntaxTree.GetTextLineExtentAtPosition(location.End);
            var lineStartIndex = endLineExtent.Extent.Start;
            var length = location.EndLinePosition.Character;

            var text = syntaxTree.SourceText.Substring(lineStartIndex, length);
            var column = text.GetColumnForOffset(editorSettings.TabSize, length);

            return column;
        }

        public static int GetStartColumn(this SyntaxTree syntaxTree, Location location, EditorSettings editorSettings) {

            var startLineExtent = syntaxTree.GetTextLineExtentAtPosition(location.Start);
            var lineStartIndex = startLineExtent.Extent.Start;
            var length = location.StartLinePosition.Character;

            var text = syntaxTree.SourceText.Substring(lineStartIndex, length);
            var column = text.GetColumnForOffset(editorSettings.TabSize, length);

            return column;
        }

        public static string GetLineIndent(this SyntaxTree syntaxTree, TextLineExtent lineExtent, EditorSettings editorSettings) {

            var line = syntaxTree.SourceText.Substring(lineExtent.Extent.Start, lineExtent.Extent.Length);

            var startColumn = line.GetSignificantColumn(editorSettings.TabSize);

            return new String(' ', startColumn);
        }

        public static SyntaxToken FirstNoneWhitespaceToken(this SyntaxTree syntaxTree, TextExtent extent) {
            return syntaxTree.Tokens[extent]
                             .SkipWhile(token => token.Type == SyntaxTokenType.Whitespace)
                             .FirstOrDefault();
        }

        public static int ColumnsBetweenKeywordAndIdentifier(this SyntaxTree syntaxTree, INodeSymbol node, string newKeyword, EditorSettings editorSettings) {

            var locations = KeywordAndIdentifierFinder.Find(node.Syntax);
            if (locations == null) {
                return 1;
            }

            var oldOffset = syntaxTree.ColumnsBetweenLocations(locations.Item1, locations.Item2, editorSettings);

            var oldLength = locations.Item1.Length;
            var newLength = newKeyword?.Length ?? oldLength;
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