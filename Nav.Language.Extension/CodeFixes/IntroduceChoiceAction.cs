#region Using Directives

using System;
using System.Threading;

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;
using Pharmatechnik.Nav.Language.Extension.Common;
using Pharmatechnik.Nav.Language.Extension.Images;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.CodeFixes {
    
    class IntroduceChoiceAction : CodeFixAction {

        public IntroduceChoiceAction(INodeReferenceSymbol nodeReference,
                                     CodeFixActionsParameter parameter, 
                                     CodeFixActionContext context): base(context, parameter) {

            NodeReference = nodeReference;
        }

        public INodeReferenceSymbol NodeReference { get; }
        public override Span? ApplicableToSpan   => GetSnapshotSpan(NodeReference);
        public override string DisplayText       => "Introduce choice";
        public override ImageMoniker IconMoniker => KnownMonikers.InsertClause;

        public override void Invoke(CancellationToken cancellationToken) {

            var containingTask = NodeReference.Declaration?.ContainingTask;
            if (containingTask == null) {
                return;
            }
            
            var declaredNames = GetDeclaredNodeNames(containingTask);

            string Validator(string validationText) {

                validationText = validationText?.Trim();

                if (!SyntaxFacts.IsValidIdentifier(validationText)) {
                    return "Invalid identifier";
                }

                if (declaredNames.Contains(validationText)) {
                    return $"A node with the name '{validationText}' is already declared";
                }

                return null;
            }

            var choiceName = Context.InputDialogService.ShowDialog(
                promptText    : "Name:",
                title         : DisplayText,
                defaultResonse: $"Choice_{NodeReference.Name}",
                iconMoniker   : ImageMonikers.ChoiceNode,
                validator     : Validator
            )?.Trim();

            if (String.IsNullOrEmpty(choiceName)) {
                return;
            }

            Apply(choiceName);
        }

        void Apply(string choiceName) {

            var undoDescription = $"{DisplayText} '{choiceName}'";
            var waitMessage     = $"{undoDescription}...";
            
            ApplyTextEdits(undoDescription, waitMessage, textEdit => {

                var edge       = NodeReference.Edge;
                var edgeMode   = edge.EdgeMode;
                var nodeSymbol = NodeReference.Declaration;

                // ReSharper disable once PossibleNullReferenceException Check ist schon früher passiert
                var nodeDeclarationLine = textEdit.Snapshot.GetLineFromPosition(nodeSymbol.Start);
                var nodeTransitionLine  = textEdit.Snapshot.GetLineFromPosition(NodeReference.End);

                var choiceDeclaration = $"{GetFirstWhitespace(nodeDeclarationLine)}{SyntaxFacts.ChoiceKeyword}{WhiteSpaceBetweenChoiceKeywordAndIdentifier(nodeSymbol)}{choiceName}{SyntaxFacts.Semicolon}";
                var choiceTransition  = $"{GetFirstWhitespace(nodeTransitionLine)}{choiceName}{WhiteSpaceBetweenSourceAndEdgeMode(edge, choiceName)}{edge.EdgeMode?.Name}{WhiteSpaceBetweenEdgeModeAndTarget(edge)}{NodeReference.Name}{SyntaxFacts.Semicolon}";

                textEdit.Insert(nodeDeclarationLine.End, $"{GetNewLineCharacter()}{choiceDeclaration}");
               
                //// Die Node Reference wird nun umgebogen auf die choice
                ReplaceSymbol(textEdit, NodeReference, choiceName);
                ReplaceSymbol(textEdit, edgeMode, SyntaxFacts.GoToEdgeKeyword);
                
                textEdit.Insert(nodeTransitionLine.End, $"{GetNewLineCharacter()}{choiceTransition}");   
            });
        }

        string GetFirstWhitespace(ITextSnapshotLine line) {

            var end = line.GetFirstNonWhitespacePosition();
            if (end == null) {
                return String.Empty;
            }

            var startColumn = line.GetColumnForOffset(GetTabSize(), end.Value - line.Start);

            return new String(' ', startColumn);
        }

        string WhiteSpaceBetweenSourceAndEdgeMode(IEdge edge, string newSourceName) {

            if (edge.Source == null || edge.EdgeMode == null) {
                return " ";
            }
            
            var sourceText = edge.ContainingTask.Syntax.SyntaxTree.SourceText;
            var oldOffset  = ColumnsBetweenLocations(sourceText, edge.Source.Location, edge.EdgeMode.Location, GetTabSize());

            var oldLength = edge.Source.Location.Length;
            var newLength = newSourceName.Length;
            var offset    = Math.Max(1, oldOffset + oldLength - newLength);

            return new String(' ', offset);
        }

        string WhiteSpaceBetweenEdgeModeAndTarget(IEdge edge) {

            if (edge.EdgeMode == null || edge.Target == null) {
                return " ";
            }
            var sourceText=edge.ContainingTask.Syntax.SyntaxTree.SourceText;
            var offset = ColumnsBetweenLocations(sourceText, edge.EdgeMode.Location, edge.Target.Location, GetTabSize());
            return new String(' ', offset);
        }

        string WhiteSpaceBetweenChoiceKeywordAndIdentifier(INodeSymbol referenceNode) {
            
            var locations= KeywordAndIdentifierFinder.Find(referenceNode.Syntax);
            if (locations == null) {
                return " ";
            }

            var sourceText = referenceNode.ContainingTask.Syntax.SyntaxTree.SourceText;
            var oldOffset = ColumnsBetweenLocations(sourceText, locations.Item1, locations.Item2, GetTabSize());

            var oldLength = locations.Item1.Length;
            var newLength = SyntaxFacts.ChoiceKeyword.Length;
            var offset    = Math.Max(1, oldOffset + oldLength - newLength);

            return new String(' ', offset);            
        }

        int ColumnsBetweenLocations(string sourceText, Location location1, Location location2, int tabSize) {

            if (location1 == null || location2 == null) {
                return 0;
            }

            int spaceCount = 1;
            if(location1.EndLine != location2.StartLine) {
                // Locations in unterschiedliche Zeilen
                var column = GetStartColumn(sourceText, location2, tabSize);
                spaceCount = column;
            } else {
                // Locations in selber Zeile
                var startColumn = GetEndColumn(sourceText, location1, tabSize);
                var endColumn   = GetStartColumn(sourceText, location2, tabSize);

                spaceCount = Math.Max(1, endColumn - startColumn);
            }

            return Math.Max(1, spaceCount);
        }

        int GetEndColumn(string sourceText, Location location, int tabSize) {

            var lineStartIndex = location.End - location.EndLinePosition.Character;
            var length         = location.EndLinePosition.Character;
            var text           = sourceText.Substring(lineStartIndex, length);
            var column         = text.GetColumnForOffset(tabSize, length);
            return column;
        }

        int GetStartColumn(string sourceText, Location location, int tabSize) {

            var lineStartIndex = location.Start - location.StartLinePosition.Character;
            var length         = location.StartLinePosition.Character;
            var text           = sourceText.Substring(lineStartIndex, length);
            var column         = text.GetColumnForOffset(tabSize, length);
            return column;
        }

        sealed class KeywordAndIdentifierFinder: SyntaxNodeVisitor<Tuple<Location, Location>> {

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