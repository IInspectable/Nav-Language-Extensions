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

            var newChoiceName = Context.InputDialogService.ShowDialog(
                promptText    : "Name:",
                title         : "Introduce choice",
                defaultResonse: $"Choice_{NodeReference.Name}",
                iconMoniker   : ImageMonikers.ChoiceNode,
                validator     : Validator
            )?.Trim();

            if (String.IsNullOrEmpty(newChoiceName)) {
                return;
            }

            Apply(newChoiceName);
        }

        void Apply(string newChoiceName) {

            var undoDescription = DisplayText;
            var waitMessage     = $"Introduce choice 'newChoiceName'...";
            
            ApplyTextEdits(undoDescription, waitMessage, textEdit => {

                
                // TODO
                //// Die Choice Deklaration
                //RenameSymbol(textEdit, NodeReference, newChoiceName);
               //
               //// Die Choice-Referenzen auf der "linken Seite"
               //foreach (var transition in ChoiceSymbol.Outgoings) {
               //    RenameSymbol(textEdit, transition.Source, newChoiceName);
               //}
               //
               //// Die Choice-Referenzen auf der "rechten Seite"
               //foreach (var transition in ChoiceSymbol.Incomings) {
               //    RenameSymbol(textEdit, transition.Target, newChoiceName);
               //}
            });
        }

        //void RenameSymbol(ITextEdit textEdit, ISymbol symbol, string name) {

        //    if (symbol == null || symbol.Name==name) {
        //        return;
        //    }

        //    var replaceSpan = GetTextEditSpan(textEdit, symbol.Location);
        //    textEdit.Replace(replaceSpan, name);
        //}        
    }
}