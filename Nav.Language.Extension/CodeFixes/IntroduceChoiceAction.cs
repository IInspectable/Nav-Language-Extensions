#region Using Directives

using System;
using System.Threading;

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;

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
                // TODO Tabsize
                var transition = NodeReference.Transition;
                var nodeSymbol = NodeReference.Declaration;

                var choiceDeclaration = $"{GetTabInSpaces()}{SyntaxFacts.ChoiceKeyword} {choiceName}{SyntaxFacts.Semicolon}";
                var choiceTransition  = $"{GetTabInSpaces()}{choiceName} {transition.EdgeMode?.Name} {NodeReference.Name}{SyntaxFacts.Semicolon}";

                // ReSharper disable once PossibleNullReferenceException Check ist schon früher passiert
                var choiceDeclaraionLine=textEdit.Snapshot.GetLineFromPosition(nodeSymbol.Start);
                textEdit.Insert(choiceDeclaraionLine.End, $"{GetNewLineCharacter()}{choiceDeclaration}");
               
                //// Die Node Reference wird nun umgebogen auf die choice
                ReplaceSymbol(textEdit, NodeReference, choiceName);

                var choiceTransitionLine = textEdit.Snapshot.GetLineFromPosition(NodeReference.End);
                textEdit.Insert(choiceTransitionLine.End, $"{GetNewLineCharacter()}{choiceTransition}");   
            });
        }        
    }
}