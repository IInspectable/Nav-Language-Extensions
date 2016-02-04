#region Using Directives

using System.ComponentModel.Composition;

using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using Pharmatechnik.Nav.Language.Extension.Common;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.GoToDefinition {

    [Export(typeof(IMouseProcessorProvider))]
    [Name("Nav/GoToDefinitionMouseProcessorProvider")]
    [ContentType(NavLanguageContentDefinitions.ContentType)]
    [TextViewRole(PredefinedTextViewRoles.Editable)]
    sealed class GoToDefinitionMouseProcessorProvider : IMouseProcessorProvider {

        readonly TextViewConnectionListener _textViewConnectionListener;
        readonly IViewTagAggregatorFactoryService _viewTagAggregatorFactoryService;

        [ImportingConstructor]
        public GoToDefinitionMouseProcessorProvider(TextViewConnectionListener textViewConnectionListener,
                                                IViewTagAggregatorFactoryService viewTagAggregatorFactoryService) {
            _textViewConnectionListener      = textViewConnectionListener;
            _viewTagAggregatorFactoryService = viewTagAggregatorFactoryService;
        }

        public IMouseProcessor GetAssociatedProcessor(IWpfTextView textView) {
            return GoToDefinitionMouseProcessor.GetMouseProcessorForView(textView, _textViewConnectionListener, _viewTagAggregatorFactoryService);
        }
    }
}