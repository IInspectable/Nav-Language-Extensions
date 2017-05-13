#region Using Directives

using System;
using System.Windows;

using Microsoft.VisualStudio.Text;

#endregion

namespace Pharmatechnik.Nav.Language.Extension {

    abstract class SemanticModelServiceDependent: IDisposable {

        readonly SemanticModelService _semanticModelService;

        protected SemanticModelServiceDependent(ITextBuffer textBuffer) {

            TextBuffer = textBuffer;

            _semanticModelService = SemanticModelService.GetOrCreateSingelton(textBuffer);

            WeakEventManager<SemanticModelService, EventArgs>.AddHandler(_semanticModelService, nameof(SemanticModelService.SemanticModelChanging), OnSemanticModelChanging);
            WeakEventManager<SemanticModelService, SnapshotSpanEventArgs>.AddHandler(_semanticModelService, nameof(SemanticModelService.SemanticModelChanged),  OnSemanticModelChanged);
        }
        
        public virtual void Dispose() {
            WeakEventManager<SemanticModelService, EventArgs>.RemoveHandler(_semanticModelService, nameof(SemanticModelService.SemanticModelChanging), OnSemanticModelChanging);
            WeakEventManager<SemanticModelService, SnapshotSpanEventArgs>.RemoveHandler(_semanticModelService, nameof(SemanticModelService.SemanticModelChanged), OnSemanticModelChanged);
        }

        protected ITextBuffer TextBuffer { get; }

        protected SemanticModelService SemanticModelService {
            get { return _semanticModelService; }
        }

        protected virtual void OnSemanticModelChanging(object sender, EventArgs e) {
        }

        protected virtual void OnSemanticModelChanged(object sender, SnapshotSpanEventArgs e) {            
        }        
    }
}