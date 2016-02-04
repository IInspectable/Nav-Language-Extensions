#region Using Directives

using System.ComponentModel.Composition;

using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using Pharmatechnik.Nav.Language.Extension.Common;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.GoToDefinition {

    [ContentType(NavLanguageContentDefinitions.ContentType)]
    [Name("Nav/NavigateToKeyProcessorProvider")]
    [Export(typeof(IKeyProcessorProvider))]
    [TextViewRole(PredefinedTextViewRoles.Interactive)]
    internal sealed class GoToDefinitionKeyProcessorProvider : IKeyProcessorProvider {

        readonly TextViewConnectionListener _textViewConnectionListener;

        [ImportingConstructor]
        public GoToDefinitionKeyProcessorProvider(TextViewConnectionListener textViewConnectionListener) {
            _textViewConnectionListener = textViewConnectionListener;
        }

        public KeyProcessor GetAssociatedProcessor(IWpfTextView textView) {
            return GoToDefinitionKeyProcessor.GetKeyProcessorForView(textView, _textViewConnectionListener);
        }
    }
}