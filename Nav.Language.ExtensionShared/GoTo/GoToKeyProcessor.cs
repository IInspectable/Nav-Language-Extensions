#region Using Directives

using System.Windows.Input;
using Microsoft.VisualStudio.Text.Editor;
using Pharmatechnik.Nav.Language.Extension.Common;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.GoTo; 

sealed class GoToKeyProcessor: KeyProcessor {

    readonly ModifierKeyState _keyState;

    GoToKeyProcessor(IWpfTextView textView, TextViewConnectionListener textViewConnectionListener) {
        _keyState = ModifierKeyState.GetStateForView(textView, textViewConnectionListener);
        textViewConnectionListener.AddDisconnectAction(textView, RemoveKeyProcessorForView);
    }

    public static GoToKeyProcessor GetKeyProcessorForView(IWpfTextView textView, TextViewConnectionListener textViewConnectionListener) {
        return textView.Properties.GetOrCreateSingletonProperty(() => new GoToKeyProcessor(textView, textViewConnectionListener));
    }

    void RemoveKeyProcessorForView(IWpfTextView textView) {
        textView.Properties.RemoveProperty(GetType());
    }

    public override void PreviewKeyDown(KeyEventArgs args) {
        _keyState.UpdateState();
    }

    public override void PreviewKeyUp(KeyEventArgs args) {
        _keyState.UpdateState();
    }        
}