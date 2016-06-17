#region Using Directives

using System.ComponentModel.Composition;

using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;

using Pharmatechnik.Nav.Language.Extension.Common;
using Pharmatechnik.Nav.Language.Extension.GoToLocation;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.GoTo {

    [Export(typeof(IMouseProcessorProvider))]
    [Name("Nav/" + nameof(GoToMouseProcessorProvider))]
    [ContentType(NavLanguageContentDefinitions.ContentType)]
    [TextViewRole(PredefinedTextViewRoles.Editable)]
    sealed class GoToMouseProcessorProvider : IMouseProcessorProvider {

        readonly TextViewConnectionListener _textViewConnectionListener;
        readonly IViewTagAggregatorFactoryService _viewTagAggregatorFactoryService;
        readonly GoToLocationService _goToLocationService;

        [ImportingConstructor]
        public GoToMouseProcessorProvider(TextViewConnectionListener textViewConnectionListener,
                                          IViewTagAggregatorFactoryService viewTagAggregatorFactoryService,
                                          GoToLocationService goToLocationService) {

            _textViewConnectionListener      = textViewConnectionListener;
            _viewTagAggregatorFactoryService = viewTagAggregatorFactoryService;
            _goToLocationService             = goToLocationService;
        }

        public IMouseProcessor GetAssociatedProcessor(IWpfTextView textView) {
            return GoToMouseProcessor.GetMouseProcessorForView(textView, _textViewConnectionListener, _viewTagAggregatorFactoryService, _goToLocationService);
        }
    }
}