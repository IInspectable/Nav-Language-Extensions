#region Using Directives

using System.ComponentModel.Composition;

using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using Pharmatechnik.Nav.Language.Extension.Common;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.GoTo {

    [Export(typeof(IMouseProcessorProvider))]
    [Name("Nav/" + nameof(GoToMouseProcessorProvider))]
    [ContentType(NavLanguageContentDefinitions.ContentType)]
    [TextViewRole(PredefinedTextViewRoles.Editable)]
    sealed class GoToMouseProcessorProvider : IMouseProcessorProvider {

        readonly TextViewConnectionListener _textViewConnectionListener;
        readonly IViewTagAggregatorFactoryService _viewTagAggregatorFactoryService;

        [ImportingConstructor]
        public GoToMouseProcessorProvider(TextViewConnectionListener textViewConnectionListener,
                                          IViewTagAggregatorFactoryService viewTagAggregatorFactoryService) {
            _textViewConnectionListener      = textViewConnectionListener;
            _viewTagAggregatorFactoryService = viewTagAggregatorFactoryService;
        }

        public IMouseProcessor GetAssociatedProcessor(IWpfTextView textView) {
            return GoToMouseProcessor.GetMouseProcessorForView(textView, _textViewConnectionListener, _viewTagAggregatorFactoryService);
        }
    }
}