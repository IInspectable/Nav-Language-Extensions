#region Using Directives

using System;
using Microsoft.VisualStudio.Text;
using Pharmatechnik.Nav.Language.Extension.Common;

#endregion

namespace Pharmatechnik.Nav.Language.Extension {

    abstract class SemanticModelServiceDependent: IDisposable {

        readonly TextBufferScopedValue<SemanticModelService> _semanticModelServiceSingelton;

        protected SemanticModelServiceDependent(ITextBuffer textBuffer) {

            TextBuffer = textBuffer;

            _semanticModelServiceSingelton = SemanticModelService.GetOrCreateSingelton(textBuffer);

            SemanticModelService.SemanticModelChanging += OnSemanticModelChanging;
            SemanticModelService.SemanticModelChanged  += OnSemanticModelChanged;
        }
        
        public virtual void Dispose() {

            SemanticModelService.SemanticModelChanging -= OnSemanticModelChanging;
            SemanticModelService.SemanticModelChanged  -= OnSemanticModelChanged;

            _semanticModelServiceSingelton.Dispose();
        }

        protected ITextBuffer TextBuffer { get; }

        protected SemanticModelService SemanticModelService {
            get { return _semanticModelServiceSingelton.Value; }
        }

        protected virtual void OnSemanticModelChanging(object sender, EventArgs e) {
        }

        protected virtual void OnSemanticModelChanged(object sender, SnapshotSpanEventArgs e) {            
        }        
    }
}