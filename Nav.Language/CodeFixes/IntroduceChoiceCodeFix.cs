#region Using Directives

using System;
using System.Linq;
using System.Collections.Generic;

using Pharmatechnik.Nav.Language.Text;

#endregion

namespace Pharmatechnik.Nav.Language.CodeFixes {

    public class IntroduceChoiceCodeFix: CodeFix {

        public IntroduceChoiceCodeFix(CodeGenerationUnit codeGenerationUnit, INodeReferenceSymbol nodeReference): base(codeGenerationUnit) {            
            NodeReference  = nodeReference ?? throw new ArgumentNullException(nameof(nodeReference));
        }

        public INodeReferenceSymbol NodeReference { get; }
        public ITaskDefinitionSymbol ContainingTask => NodeReference.Declaration?.ContainingTask;
        
        public override bool CanApplyFix() {

            return NodeReference.Type == NodeReferenceType.Target &&
                   NodeReference.Declaration   != null &&
                   NodeReference.Edge.Source   != null &&
                   NodeReference.Edge.EdgeMode != null &&
                 !(NodeReference.Declaration is IChoiceNodeSymbol);
        }

        public HashSet<string> GetUsedNodeNames() {
            return GetDeclaredNodeNames(ContainingTask);
        }

        public IList<TextChange> GetTextChanges(string choiceName, EditorSettings editorSettings) {

            if (!CanApplyFix()) {
                return Enumerable.Empty<TextChange>().ToList();
            }

            var edge       = NodeReference.Edge;
            var edgeMode   = edge.EdgeMode;
            var nodeSymbol = NodeReference.Declaration;

            // ReSharper disable once PossibleNullReferenceException Check ist schon früher passiert
            var nodeDeclarationLine = SyntaxTree.GetTextLineExtent(nodeSymbol.Start);
            var nodeTransitionLine  = SyntaxTree.GetTextLineExtent(NodeReference.End);

            var choiceDeclaration = $"{GetSignificantColumn(nodeDeclarationLine, editorSettings.TabSize)}{SyntaxFacts.ChoiceKeyword}{WhiteSpaceBetweenChoiceKeywordAndIdentifier(nodeSymbol, editorSettings.TabSize)}{choiceName}{SyntaxFacts.Semicolon}";
            var choiceTransition  = $"{GetSignificantColumn(nodeTransitionLine , editorSettings.TabSize)}{choiceName}{WhiteSpaceBetweenSourceAndEdgeMode(edge, choiceName, editorSettings.TabSize)}{edge.EdgeMode?.Name}{WhiteSpaceBetweenEdgeModeAndTarget(edge, editorSettings.TabSize)}{NodeReference.Name}{SyntaxFacts.Semicolon}";

            var textChanges = new List<TextChange?>();
            // Die Choice Deklaration: choice NeueChoice;
            textChanges.Add(NewInsert(nodeDeclarationLine.Extent.End, $"{choiceDeclaration}{editorSettings.NewLine}"));
            // Die Node Reference wird nun umgebogen auf die choice
            textChanges.Add(NewReplace(NodeReference, choiceName));
            // Die Edge der choice ist immer '-->'
            textChanges.Add(NewReplace(edgeMode, SyntaxFacts.GoToEdgeKeyword));
            // Die neue choice Transition 
            textChanges.Add(NewInsert(nodeTransitionLine.Extent.End, $"{choiceTransition}{editorSettings.NewLine}"));

            return textChanges.OfType<TextChange>().ToList();
        }

        string WhiteSpaceBetweenSourceAndEdgeMode(IEdge edge, string newSourceName, int tabSize) {

            if (edge.Source == null || edge.EdgeMode == null) {
                return " ";
            }

            var oldOffset = ColumnsBetweenLocations(edge.Source.Location, edge.EdgeMode.Location, tabSize);

            var oldLength = edge.Source.Location.Length;
            var newLength = newSourceName.Length;
            var offset    = Math.Max(1, oldOffset + oldLength - newLength);

            return new String(' ', offset);
        }

        string WhiteSpaceBetweenEdgeModeAndTarget(IEdge edge, int tabSize) {

            if (edge.EdgeMode == null || edge.Target == null) {
                return " ";
            }
            var offset     = ColumnsBetweenLocations(edge.EdgeMode.Location, edge.Target.Location, tabSize);
            return new String(' ', offset);
        }

        string WhiteSpaceBetweenChoiceKeywordAndIdentifier(INodeSymbol referenceNode, int tabSize) {

            var locations = KeywordAndIdentifierFinder.Find(referenceNode.Syntax);
            if (locations == null) {
                return " ";
            }

            var oldOffset = ColumnsBetweenLocations(locations.Item1, locations.Item2, tabSize);

            var oldLength = locations.Item1.Length;
            var newLength = SyntaxFacts.ChoiceKeyword.Length;
            var offset    = Math.Max(1, oldOffset + oldLength - newLength);

            return new String(' ', offset);
        }

        int ColumnsBetweenLocations(Location location1, Location location2, int tabSize) {

            if (location1 == null || location2 == null) {
                return 0;
            }

            int spaceCount = 1;
            if (location1.EndLine != location2.StartLine) {
                // Locations in unterschiedliche Zeilen
                var column = SyntaxTree.GetStartColumn(location2, tabSize);
                spaceCount = column;
            }
            else {
                // Locations in selber Zeile
                var startColumn = SyntaxTree.GetEndColumn(location1, tabSize);
                var endColumn   = SyntaxTree.GetStartColumn(location2, tabSize);

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