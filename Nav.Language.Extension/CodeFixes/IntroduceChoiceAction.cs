#region Using Directives

using System;
using System.Linq;
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

                var choiceDeclaration = $"{GetFirstWhitespace(nodeDeclarationLine)}{SyntaxFacts.ChoiceKeyword}{WhiteSpaceBetweenKeywordAndIdentifier(nodeSymbol)}{choiceName}{SyntaxFacts.Semicolon}";
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
            
            var sourceLocation = edge.Source?.Location;
            var edgeLocation   = edge.EdgeMode?.Location;

            if (sourceLocation == null || edgeLocation == null) {
                return " ";
            }
            
            var sourceText = edge.ContainingTask.Syntax.SyntaxTree.SourceText;
            var oldOffset = SpaceBetweenLocations(sourceText, sourceLocation, edgeLocation, GetTabSize()).Length;

            var oldLength = edge.Source.Location.Length;
            var newLength = newSourceName.Length;
            var offset = Math.Max(1, oldOffset + oldLength - newLength);

            return new String(' ', offset);
        }

        string WhiteSpaceBetweenEdgeModeAndTarget(IEdge edge) {

            var location1 = edge.EdgeMode?.Location;
            var location2 = edge.Target?.Location;

            if (location1 == null || location2 == null) {
                return " ";
            }
            var sourceText=edge.ContainingTask.Syntax.SyntaxTree.SourceText;
            return SpaceBetweenLocations(sourceText, location1, location2, GetTabSize());
        }

        string WhiteSpaceBetweenKeywordAndIdentifier(INodeSymbol choice) {

            var keyword = choice.Syntax.ChildTokens().First();
            var identifier = keyword.NextToken();// choice.Syntax.ChildTokens().FirstOrMissing(SyntaxTokenClassification.Identifier);
            if(keyword.IsMissing || identifier.IsMissing) {
                return " ";
            }
            // TODO Berechnen
            return " ";
            //var startIndex    = keyword.End;
            //var endIndexIndex = identifier.Start;

            //return choice.Syntax.SyntaxTree.SourceText.Substring(startIndex, endIndexIndex - startIndex);
        }

        // TODO Columns Between Locations?
        string SpaceBetweenLocations(string sourceText, Location location1, Location location2, int tabSize) {
            // TODO Missing?
            if (location1 == null || location2 == null) {
                return String.Empty;
            }

            int spaceCount = 1;
            if(location1.EndLine != location2.StartLine) {
                // Symbols in unterschiedliche Zeilen                
                var column = GetStartColumn(sourceText, location2, tabSize);
                spaceCount = column;
            } else {
                var startColumn = GetEndColumn(sourceText, location1, tabSize);
                var endColumn   = GetStartColumn(sourceText, location2, tabSize);

                spaceCount = Math.Max(1, endColumn - startColumn);
            }

            return new String(' ', Math.Max(1, spaceCount));
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
    }
}