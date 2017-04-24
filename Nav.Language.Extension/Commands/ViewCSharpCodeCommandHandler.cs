#region Using Directives

using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.Composition;

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;

using Pharmatechnik.Nav.Language.CodeGen;
using Pharmatechnik.Nav.Language.Extension.Common;
using Pharmatechnik.Nav.Language.Extension.GoToLocation;
using Pharmatechnik.Nav.Language.Extension.GoToLocation.Provider;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Commands {

    // TODO Code Review
    [ExportCommandHandler(CommandHandlerNames.ViewCSharpCodeCommandHandler, NavLanguageContentDefinitions.ContentType)]
    class ViewCSharpCodeCommandHandler: ICommandHandler<ViewCodeCommandArgs> {
        readonly GoToLocationService _goToLocationService;

        [ImportingConstructor]
        public ViewCSharpCodeCommandHandler(GoToLocationService goToLocationService) {
            _goToLocationService = goToLocationService;
        }
        public CommandState GetCommandState(ViewCodeCommandArgs args, Func<CommandState> nextHandler) {
            return CommandState.Available;
        }

        public async void ExecuteCommand(ViewCodeCommandArgs args, Action nextHandler) {

            var semanticModelService=SemanticModelService.TryGet(args.SubjectBuffer);
            var semanticModelResult = semanticModelService?.SemanticModelResult;
            if (semanticModelResult == null) {
                nextHandler();
                return;
            }

            var navigateToTagSpan = GetGoToCodeTagSpanAtCaretPosition(semanticModelResult, args);
            if (navigateToTagSpan == null) {
                nextHandler();
                return;
            }

            var caretSpan = args.TextView.Caret.Position.BufferPosition.ExtendToLength1();
            var caretGeometry = args.TextView.TextViewLines.GetTextMarkerGeometry(caretSpan);
            if (caretGeometry == null) {
                nextHandler();
                return;
            }
            
            var placementRectangle = caretGeometry.Bounds;
            placementRectangle.Offset(-args.TextView.ViewportLeft, -args.TextView.ViewportTop);

            await _goToLocationService.GoToLocationInPreviewTabAsync(
                originatingTextView: args.TextView,
                placementRectangle : placementRectangle,
                provider           : navigateToTagSpan.Tag.Provider);

        }

        TagSpan<GoToTag> GetGoToCodeTagSpanAtCaretPosition(SemanticModelResult semanticModelResult, ViewCodeCommandArgs args) {
            
            var tags = BuildTagSpans(semanticModelResult, args.SubjectBuffer)
                                .OrderBy(tag => tag.Span.Start.Position)
                                .ToList();

            var caretPosition = args.TextView.Caret.Position.BufferPosition;
            var navigateToTagSpan = tags.FirstOrDefault(tagSpan => caretPosition >= tagSpan.Span.Start.Position && caretPosition <= tagSpan.Span.End.Position);

            if (navigateToTagSpan != null) {
                return navigateToTagSpan;
            }

            navigateToTagSpan = tags.FirstOrDefault(tagSpan => caretPosition < tagSpan.Span.Start.Position && caretPosition < tagSpan.Span.End.Position);
            if (navigateToTagSpan == null) {
                // Den letzten Eintrag wählen
                navigateToTagSpan = tags.LastOrDefault();
            }

            return navigateToTagSpan;
        }

        IEnumerable<TagSpan<GoToTag>> BuildTagSpans(SemanticModelResult semanticModelResult, ITextBuffer subjectBuffer) {

            foreach (var taskDeclaration in semanticModelResult.CodeGenerationUnit.TaskDeclarations.Where(td=>!td.IsIncluded && td.Origin==TaskDeclarationOrigin.TaskDeclaration)) {
                var codeModel = new TaskDeclarationCodeModel(taskDeclaration);
                var provider  = new TaskIBeginInterfaceDeclarationCodeFileLocationInfoProvider(subjectBuffer, codeModel);

                yield return CreateTagSpan(semanticModelResult, taskDeclaration.Syntax?.GetLocation(), provider);
            }

            foreach (var taskDefinition in semanticModelResult.CodeGenerationUnit.TaskDefinitions) {
                var codeModel = new TaskCodeModel(taskDefinition);
                var provider  = new TaskDeclarationCodeFileLocationInfoProvider(subjectBuffer, codeModel);

                yield return CreateTagSpan(semanticModelResult, taskDefinition.Syntax.GetLocation(), provider);
            }
        }

        TagSpan<GoToTag> CreateTagSpan(SemanticModelResult semanticModelResult, Location sourceLocation, ILocationInfoProvider provider) {
            var tagSpan = new SnapshotSpan(semanticModelResult.Snapshot, sourceLocation.Start, sourceLocation.Length);
            var tag     = new GoToTag(provider);

            return new TagSpan<GoToTag>(tagSpan, tag);
        }
    }
}