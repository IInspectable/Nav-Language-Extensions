#region Using Directives

using System.ComponentModel.Composition;

using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.GoToDefinition {

    [Export(typeof(IWpfTextViewCreationListener))]
    [ContentType(NavLanguageContentDefinitions.ContentType)]
    [TextViewRole(PredefinedTextViewRoles.Interactive)]
    sealed class GoToDefinitionAdornerTextViewCreationListener : IWpfTextViewCreationListener {
        
        #pragma warning disable 649, 169

        [Export(typeof(AdornmentLayerDefinition))]
        [Name(GoToDefinitionAdorner.AdornerName)]
        [Order(After = PredefinedAdornmentLayers.Text)]
        // ReSharper disable once UnassignedField.Local
        private AdornmentLayerDefinition _editorAdornmentLayer;

        #pragma warning restore 649, 169
        
        public void TextViewCreated(IWpfTextView textView) {
            GoToDefinitionAdorner.GetOrCreate(textView);
        }        
    }
}