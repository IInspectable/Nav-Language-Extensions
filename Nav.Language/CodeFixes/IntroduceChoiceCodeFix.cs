#region Using Directives

using System;
using System.Linq;
using System.Collections.Generic;

using Pharmatechnik.Nav.Language.Text;

#endregion

namespace Pharmatechnik.Nav.Language.CodeFixes {

    public class IntroduceChoiceCodeFix {

        readonly CodeGenerationUnit _codeGenerationUnit;

        // TODO INodeReferenceSymbol gleich mitgeben? Kostet ja nichts... Und dann Interface mit CanApply Methode ohne Parameter...
        public IntroduceChoiceCodeFix(CodeGenerationUnit codeGenerationUnit) {
            _codeGenerationUnit = codeGenerationUnit ?? throw new ArgumentNullException(nameof(codeGenerationUnit));
        }

        public bool CanApplyFix(INodeReferenceSymbol nodeReference) {
            // TODO überprüfen, ob Node Reference in CodeGenerationUnit?
            return nodeReference.Type == NodeReferenceType.Target &&
                   nodeReference.Declaration != null &&
                   nodeReference.Edge.Source != null &&
                   nodeReference.Edge.EdgeMode != null &&
                   !(nodeReference.Declaration is IChoiceNodeSymbol);
        }

        public IList<TextChange> GetTextChanges(INodeReferenceSymbol nodeReference, string choiceName, int tabSize, string newLineCharacter) {

            if (!CanApplyFix(nodeReference)) {
                return Enumerable.Empty<TextChange>().ToList();
            }
            
            var syntaxTree = _codeGenerationUnit.Syntax.SyntaxTree;

            var edge       = nodeReference.Edge;
            var edgeMode   = edge.EdgeMode;
            var nodeSymbol = nodeReference.Declaration;

            // ReSharper disable once PossibleNullReferenceException Check ist schon früher passiert
            var nodeDeclarationLine = syntaxTree.GetTextLineExtent(nodeSymbol.Start);
            var nodeTransitionLine  = syntaxTree.GetTextLineExtent(nodeReference.End);

            var choiceDeclaration = $"{GetSignificantColumn(syntaxTree, nodeDeclarationLine, tabSize)}{SyntaxFacts.ChoiceKeyword}{WhiteSpaceBetweenChoiceKeywordAndIdentifier(nodeSymbol, tabSize)}{choiceName}{SyntaxFacts.Semicolon}";
            var choiceTransition  = $"{GetSignificantColumn(syntaxTree, nodeTransitionLine, tabSize)}{choiceName}{WhiteSpaceBetweenSourceAndEdgeMode(edge, choiceName, tabSize)}{edge.EdgeMode?.Name}{WhiteSpaceBetweenEdgeModeAndTarget(edge, tabSize)}{nodeReference.Name}{SyntaxFacts.Semicolon}";

            var textChanges = new List<TextChange?>();
            // Die Choice Deklaration: choice NeueChoice;
            textChanges.Add(NewInsert(nodeDeclarationLine.Extent.End, $"{choiceDeclaration}{newLineCharacter}"));
            // Die Node Reference wird nun umgebogen auf die choice
            textChanges.Add(NewReplace(nodeReference, choiceName));
            // Die Edge der choice ist immer '-->'
            textChanges.Add(NewReplace(edgeMode, SyntaxFacts.GoToEdgeKeyword));
            // Die neue choice Transition 
            textChanges.Add(NewInsert(nodeTransitionLine.Extent.End, $"{choiceTransition}{newLineCharacter}"));

            return textChanges.OfType<TextChange>().ToList();
        }

        // TODO Infrastrukur in Basisklasse
        static TextChange? NewReplace(ISymbol symbol, string newName) {
            if (symbol == null || symbol.Name == newName) {
                return null;
            }
            return new TextChange(symbol.Location.Extent, newName);
        }

        public static TextChange? NewInsert(int position, string newText) {
            return new TextChange(TextExtent.FromBounds(position, position), newText);
        }
        
        static string GetSignificantColumn(SyntaxTree syntaxTree, TextLineExtent lineExtent, int tabSize) {

            var line = syntaxTree.SourceText.Substring(lineExtent.Extent.Start, lineExtent.Extent.Length);

            var startColumn = line.GetSignificantColumn(tabSize);

            return new String(' ', startColumn);
        }

        static string WhiteSpaceBetweenSourceAndEdgeMode(IEdge edge, string newSourceName, int tabSize) {

            if (edge.Source == null || edge.EdgeMode == null) {
                return " ";
            }

            var syntaxTree = edge.ContainingTask.Syntax.SyntaxTree;
            var oldOffset = ColumnsBetweenLocations(syntaxTree, edge.Source.Location, edge.EdgeMode.Location, tabSize);

            var oldLength = edge.Source.Location.Length;
            var newLength = newSourceName.Length;
            var offset    = Math.Max(1, oldOffset + oldLength - newLength);

            return new String(' ', offset);
        }

        static string WhiteSpaceBetweenEdgeModeAndTarget(IEdge edge, int tabSize) {

            if (edge.EdgeMode == null || edge.Target == null) {
                return " ";
            }
            var syntaxTree = edge.ContainingTask.Syntax.SyntaxTree;
            var offset     = ColumnsBetweenLocations(syntaxTree, edge.EdgeMode.Location, edge.Target.Location, tabSize);
            return new String(' ', offset);
        }

        static string WhiteSpaceBetweenChoiceKeywordAndIdentifier(INodeSymbol referenceNode, int tabSize) {

            var locations = KeywordAndIdentifierFinder.Find(referenceNode.Syntax);
            if (locations == null) {
                return " ";
            }

            var syntaxTree = referenceNode.ContainingTask.Syntax.SyntaxTree;
            var oldOffset  = ColumnsBetweenLocations(syntaxTree, locations.Item1, locations.Item2, tabSize);

            var oldLength = locations.Item1.Length;
            var newLength = SyntaxFacts.ChoiceKeyword.Length;
            var offset    = Math.Max(1, oldOffset + oldLength - newLength);

            return new String(' ', offset);
        }

        static int ColumnsBetweenLocations(SyntaxTree syntaxTree, Location location1, Location location2, int tabSize) {

            if (location1 == null || location2 == null) {
                return 0;
            }

            int spaceCount = 1;
            if (location1.EndLine != location2.StartLine) {
                // Locations in unterschiedliche Zeilen
                var column = syntaxTree.GetStartColumn(location2, tabSize);
                spaceCount = column;
            }
            else {
                // Locations in selber Zeile
                var startColumn = syntaxTree.GetEndColumn(location1, tabSize);
                var endColumn = syntaxTree.GetStartColumn(location2, tabSize);

                spaceCount = Math.Max(1, endColumn - startColumn);
            }

            return Math.Max(1, spaceCount);
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