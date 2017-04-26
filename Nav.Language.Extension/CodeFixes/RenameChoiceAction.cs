#region Using Directives

using System;
using System.Threading;
using System.Collections.Generic;
using System.Collections.Immutable;

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;

using Pharmatechnik.Nav.Language.Extension.Common;
using Pharmatechnik.Nav.Language.Extension.Images;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.CodeFixes {
    
    class RenameChoiceAction: CodeFixAction {

        readonly IChoiceNodeSymbol _choiceSymbol;
        
        public RenameChoiceAction(IChoiceNodeSymbol choiceSymbol,
                                  CodeFixActionsArgs codeFixActionsArgs, 
                                  CodeFixActionContext context): base(context, codeFixActionsArgs) {
            _choiceSymbol = choiceSymbol;
        }
        
        public override void Invoke(CancellationToken cancellationToken) {

            var declaredNames = GetDeclaredNodeNames();
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

            var name = Context.InputDialogService.ShowDialog(
                promptText    : "Name:",
                title         : "Rename choice",
                defaultResonse: _choiceSymbol.Name,
                iconMoniker   : ImageMonikers.ChoiceNode,
                validator     : Validator
            )?.Trim();

            if (String.IsNullOrEmpty(name)) {
                return;
            }

            Apply(name);
        }
        
        ImmutableHashSet<string> GetDeclaredNodeNames() {
            HashSet<string> reserved = new HashSet<string>();
            foreach (var node in _choiceSymbol.ContainingTask.NodeDeclarations) {
                var name = node.Name;                
                if (!String.IsNullOrEmpty(name) && name!=_choiceSymbol.Name) {
                    reserved.Add(name);
                }
            }
            return reserved.ToImmutableHashSet();
        }

        private void Apply(string choiceName) {

            var message = $"Renaming choice '{_choiceSymbol.Name}'...";
            var title   = DisplayText;

            using (Context.WaitIndicator.StartWait(title, message, allowCancel: false))
            using (var undoTransaction = new TextUndoTransaction(title, TextView, Context.UndoHistoryRegistry, Context.EditorOperationsFactoryService))
            using (var textEdit = TextBuffer.CreateEdit()) {

                // Die Choice Deklaration
                RenameSymbol(textEdit, _choiceSymbol, choiceName);

                // Die Choice-Referenzen auf der "linken Seite"
                foreach (var transition in _choiceSymbol.Outgoings) {
                    RenameSymbol(textEdit, transition.Source, choiceName);
                }

                // Die Choice-Referenzen auf der "rechten Seite"
                foreach (var transition in _choiceSymbol.Incomings) {
                    RenameSymbol(textEdit, transition.Target, choiceName);
                }
                
                textEdit.Apply();

                undoTransaction.Commit();
            }
        }

        void RenameSymbol(ITextEdit textEdit, ISymbol symbol, string name) {

            if (symbol == null || symbol.Name==name) {
                return;
            }

            var replaceSpan = GetTextEditSpan(textEdit, symbol.Location);
            textEdit.Replace(replaceSpan, name);
        }
        
        public override string DisplayText {
            get { return $"Rename choice '{_choiceSymbol.Name}'"; }
        }
        public override ImageMoniker IconMoniker => KnownMonikers.Rename;
    }
}