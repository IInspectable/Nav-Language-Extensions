#region Using Directives

using System.Threading;
using System.Threading.Tasks;

using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Threading;

using ThreadHelper = Microsoft.VisualStudio.Shell.ThreadHelper;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.QuickInfo {

    sealed class SymbolQuickInfoSource: SemanticModelServiceDependent, IAsyncQuickInfoSource {

        public SymbolQuickInfoSource(ITextBuffer textBuffer,
                                     SyntaxQuickinfoBuilderService syntaxQuickinfoBuilderService): base(textBuffer) {

            SyntaxQuickinfoBuilderService = syntaxQuickinfoBuilderService;
        }

        public SyntaxQuickinfoBuilderService SyntaxQuickinfoBuilderService { get; }

        public async Task<QuickInfoItem> GetQuickInfoItemAsync(IAsyncQuickInfoSession session, CancellationToken cancellationToken) {

            await Task.Yield().ConfigureAwait(false);
            if (cancellationToken.IsCancellationRequested) {
                return null;
            }

            var codeGenerationUnitAndSnapshot = SemanticModelService.CodeGenerationUnitAndSnapshot;
            if (codeGenerationUnitAndSnapshot == null) {
                return null;
            }

            // Map the trigger point down to our buffer.
            SnapshotPoint? subjectTriggerPoint = session.GetTriggerPoint(codeGenerationUnitAndSnapshot.Snapshot);
            if (!subjectTriggerPoint.HasValue) {
                return null;
            }

            var triggerSymbol = codeGenerationUnitAndSnapshot.CodeGenerationUnit.Symbols.FindAtPosition(subjectTriggerPoint.Value.Position);

            if (triggerSymbol == null) {
                return null;
            }

            var location = triggerSymbol.Location;
            var applicableToSpan = codeGenerationUnitAndSnapshot.Snapshot.CreateTrackingSpan(
                location.Start,
                location.Length,
                SpanTrackingMode.EdgeExclusive);

            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            var qiContent = SymbolQuickInfoBuilder.Build(triggerSymbol, SyntaxQuickinfoBuilderService);
            if (qiContent == null) {
                return null;
            }

            return new QuickInfoItem(applicableToSpan: applicableToSpan,
                                     item: qiContent);
        }

    }

}