#region Using Directives

using System;
using System.Windows;

using Microsoft.VisualStudio.Text;

#endregion

namespace Pharmatechnik.Nav.Language.Extension {

    abstract class SemanticModelServiceDependent: IDisposable {

        protected SemanticModelServiceDependent(ITextBuffer textBuffer) {

            TextBuffer           = textBuffer;
            SemanticModelService = SemanticModelService.GetOrCreateSingelton(textBuffer);

            WeakEventManager<SemanticModelService, EventArgs>.AddHandler(SemanticModelService, nameof(SemanticModelService.SemanticModelChanging), OnSemanticModelChanging);
            WeakEventManager<SemanticModelService, SnapshotSpanEventArgs>.AddHandler(SemanticModelService, nameof(SemanticModelService.SemanticModelChanged), OnSemanticModelChanged);
        }

        public virtual void Dispose() {
            WeakEventManager<SemanticModelService, EventArgs>.RemoveHandler(SemanticModelService, nameof(SemanticModelService.SemanticModelChanging), OnSemanticModelChanging);
            WeakEventManager<SemanticModelService, SnapshotSpanEventArgs>.RemoveHandler(SemanticModelService, nameof(SemanticModelService.SemanticModelChanged), OnSemanticModelChanged);
        }

        public ITextBuffer TextBuffer { get; }

        public SemanticModelService SemanticModelService { get; }

        protected virtual void OnSemanticModelChanging(object sender, EventArgs e) {
        }

        protected virtual void OnSemanticModelChanged(object sender, SnapshotSpanEventArgs e) {
        }

    }

}