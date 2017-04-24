#region Using Directives

using System;
using System.ComponentModel.Composition;

using Microsoft.VisualStudio.Text.Tagging;

using Pharmatechnik.Nav.Language.Extension.Common;
using Pharmatechnik.Nav.Language.Extension.GoToLocation;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Commands {

    [ExportCommandHandler(CommandHandlerNames.GoToDefinitionCommandCommandHandler, NavLanguageContentDefinitions.ContentType)]
    class GoToDefinitionCommandCommandHandler: ICommandHandler<GoToDefinitionCommandArgs> {

        readonly GoToLocationService _goToLocationService;
        readonly IViewTagAggregatorFactoryService _viewTagAggregatorFactoryService;
        
        [ImportingConstructor]
        public GoToDefinitionCommandCommandHandler(IViewTagAggregatorFactoryService viewTagAggregatorFactoryService, GoToLocationService goToLocationService) {
            _goToLocationService = goToLocationService;
            _viewTagAggregatorFactoryService = viewTagAggregatorFactoryService;            
        }

        public CommandState GetCommandState(GoToDefinitionCommandArgs args, Func<CommandState> nextHandler) {
            return CommandState.Available;
        }

        public async void ExecuteCommand(GoToDefinitionCommandArgs args, Action nextHandler) {

            var tagAggregator     = _viewTagAggregatorFactoryService.CreateTagAggregator<GoToTag>(args.TextView);
            var navigateToTagSpan = args.TextView.GetGoToDefinitionTagSpanAtCaretPosition(tagAggregator);
            
            if (navigateToTagSpan == null) {
                ShellUtil.ShowInfoMessage("Cannot navigate to the symbol under the caret.");                
                return;
            }

            var placementRectangle = args.TextView.TextViewLines.GetTextMarkerGeometry(navigateToTagSpan.Span).Bounds;
            placementRectangle.Offset(-args.TextView.ViewportLeft, -args.TextView.ViewportTop);

            await _goToLocationService.GoToLocationInPreviewTabAsync(
                originatingTextView: args.TextView,
                placementRectangle : placementRectangle,
                provider           : navigateToTagSpan.Tag.Provider);

        }
    }
}
