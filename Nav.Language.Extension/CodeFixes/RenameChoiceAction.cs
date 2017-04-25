#region Using Directives

using System;
using System.Threading;
using System.Collections.Generic;
using System.Collections.Immutable;

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;

using Pharmatechnik.Nav.Language.Extension.Common;
using Pharmatechnik.Nav.Language.Extension.Utilities;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.CodeFixes {
    // TODO SemanticmodelResult durchschleifen, bzw einen "Context" einführen
    class RenameChoiceAction: CodeFixAction {

        readonly IChoiceNodeSymbol _choiceSymbol;
        readonly ITextBuffer _textBuffer;
        readonly ITextView _textView;
        readonly IWaitIndicator _waitIndicator;
        readonly ITextUndoHistoryRegistry _undoHistoryRegistry;
        readonly IEditorOperationsFactoryService _editorOperationsFactoryService;

        public RenameChoiceAction(IChoiceNodeSymbol choiceSymbol, ITextBuffer textBuffer, ITextView textView, 
                                  IWaitIndicator waitIndicator, ITextUndoHistoryRegistry undoHistoryRegistry, 
                                  IEditorOperationsFactoryService editorOperationsFactoryService) {

            _choiceSymbol                   = choiceSymbol;
            _textBuffer                     = textBuffer;
            _textView                       = textView;
            _waitIndicator                  = waitIndicator;
            _undoHistoryRegistry            = undoHistoryRegistry;
            _editorOperationsFactoryService = editorOperationsFactoryService;
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

            using (_waitIndicator.StartWait(title, message, allowCancel: false))
            using (var undoTransaction = new TextUndoTransaction(title, _textView, _undoHistoryRegistry, _editorOperationsFactoryService))
            using (var textEdit = _textBuffer.CreateEdit()) {

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