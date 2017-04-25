#region Using Directives

using System;
using System.Threading;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.VisualStudio.Text;

using Pharmatechnik.Nav.Language.Extension.Common;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.CodeFixes {
    // TODO SemanticmodelResult durchschleifen, bzw einen "Context" einführen
    class RenameChoiceAction: CodeFixAction {

        readonly IChoiceNodeSymbol _choiceSymbol;
        
        public RenameChoiceAction(IChoiceNodeSymbol choiceSymbol,
                                  CodeFixActionsArgs codeFixActionsArgs, 
                                  CodeFixActionContext context): base(context, codeFixActionsArgs) {
            _choiceSymbol = choiceSymbol;
        }
        
        public override void Invoke(CancellationToken cancellationToken) {

            var reserved = CollectReservedNames();
            //TODO Interavtive Abfrage
            var choiceName = "Foo";
            
            Apply(choiceName);
        }

        ImmutableHashSet<string> CollectReservedNames() {
            // TODO Aliase berücksichtigen!!
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

        static void RenameSymbol(ITextEdit textEdit, ISymbol symbol, string name) {
            if (symbol == null || symbol.Name==name) {
                return;
            }
            // TODO Hier mit TrackingSpans arbeiten!
            var location = symbol.Location;
            var span =new Span(location.Start, length: location.Length);
            textEdit.Replace(span, name);
        }

        public override string DisplayText {
            get { return $"Rename choice '{_choiceSymbol.Name}'"; }
        }
    }
}