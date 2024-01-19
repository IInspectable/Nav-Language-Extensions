#region Using Directives

using System;

using Microsoft.VisualStudio.Text;

#endregion

namespace Pharmatechnik.Nav.Language.Extension;

abstract class SemanticModelServiceDependent: IDisposable {

    protected SemanticModelServiceDependent(ITextBuffer textBuffer) {

        TextBuffer           = textBuffer;
        SemanticModelService = SemanticModelService.GetOrCreateSingelton(textBuffer);

        SemanticModelService.SemanticModelChanging += OnSemanticModelChanging;
        SemanticModelService.SemanticModelChanged  += OnSemanticModelChanged;
    }

    public virtual void Dispose() {
        SemanticModelService.SemanticModelChanging -= OnSemanticModelChanging;
        SemanticModelService.SemanticModelChanged  -= OnSemanticModelChanged;
    }

    public ITextBuffer TextBuffer { get; }

    public SemanticModelService SemanticModelService { get; }

    protected virtual void OnSemanticModelChanging(object sender, EventArgs e) {
    }

    protected virtual void OnSemanticModelChanged(object sender, SnapshotSpanEventArgs e) {
    }

}