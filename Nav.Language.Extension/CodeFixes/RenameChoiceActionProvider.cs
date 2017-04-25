#region Using Directives

using System.Linq;
using System.Threading;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Pharmatechnik.Nav.Language.Extension.Utilities;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.CodeFixes {
    [ExportCodeFixActionProvider(nameof(RenameChoiceActionProvider))]
    class RenameChoiceActionProvider : ICodeFixActionProvider {
        private readonly IWaitIndicator _waitIndicator;
        private readonly ITextUndoHistoryRegistry _undoHistoryRegistry;
        private readonly IEditorOperationsFactoryService _editorOperationsFactoryService;

        [ImportingConstructor]
        public RenameChoiceActionProvider(
            IWaitIndicator waitIndicator,
            ITextUndoHistoryRegistry undoHistoryRegistry,
            IEditorOperationsFactoryService editorOperationsFactoryService) {
            _waitIndicator = waitIndicator;
            _undoHistoryRegistry = undoHistoryRegistry;
            _editorOperationsFactoryService = editorOperationsFactoryService;
        }

        public IEnumerable<ISuggestedAction> GetSuggestedActions(ImmutableList<ISymbol> symbols, CodeGenerationUnit codeGenerationUnit, ITextView textView, SnapshotSpan range, CancellationToken cancellationToken) {

            var choiceNodeSymbols = ChoiceNodeFinder.FindRelatedChoiceNodes(symbols);

            return choiceNodeSymbols.Select(choiceNodeSymbol => new RenameChoiceAction(
                choiceSymbol                  : choiceNodeSymbol,
                textBuffer                    : range.Snapshot.TextBuffer, 
                textView                      : textView,
                waitIndicator                 : _waitIndicator,
                undoHistoryRegistry           : _undoHistoryRegistry,
                editorOperationsFactoryService: _editorOperationsFactoryService));              
        }

        sealed class ChoiceNodeFinder : SymbolVisitor<IChoiceNodeSymbol> {

            public static IEnumerable<IChoiceNodeSymbol> FindRelatedChoiceNodes(ImmutableList<ISymbol> symbols) {
                var finder = new ChoiceNodeFinder();
                return symbols.Select(finder.Visit).Where(choiceNodeSymbol => choiceNodeSymbol != null);                
            }

            public override IChoiceNodeSymbol VisitChoiceNodeSymbol(IChoiceNodeSymbol choiceNodeSymbol) {
                return choiceNodeSymbol;
            }

            public override IChoiceNodeSymbol VisitNodeReferenceSymbol(INodeReferenceSymbol nodeReferenceSymbol) {
                return nodeReferenceSymbol.Declaration == null ? DefaultVisit(nodeReferenceSymbol) : Visit(nodeReferenceSymbol.Declaration);
            }
        }
    }
}