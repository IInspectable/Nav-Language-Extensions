#region Using Directives

using System.Threading;
using System.Threading.Tasks;
using System.Collections.Immutable;
using System.Linq;

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion.Data;

using Pharmatechnik.Nav.Language.Extension.QuickInfo;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Completion {

    class CodeCompletionSource: AsyncCompletionSource {

        public CodeCompletionSource(QuickinfoBuilderService quickinfoBuilderService)
            : base(quickinfoBuilderService) {

        }

        public override bool TryGetApplicableToSpan(char typedChar, SnapshotPoint triggerLocation, out SnapshotSpan applicableToSpan, CancellationToken token) {

            applicableToSpan = default;

            bool IsTriggerChar() {

                return char.IsLetter(typedChar) ||
                       typedChar == '\0'        ||
                       typedChar == SyntaxFacts.OpenBracket;
            }

            if (!IsTriggerChar()) {
                return false;
            }

            return ShouldProvideCompletions(triggerLocation, out applicableToSpan);

        }

        public override Task<CompletionContext> GetCompletionContextAsync(InitialTrigger trigger, SnapshotPoint triggerLocation, SnapshotSpan applicableToSpan, CancellationToken token) {

            if (ShouldProvideCompletions(triggerLocation, out _)) {
                var completionItems = ImmutableArray.CreateBuilder<CompletionItem>();

                foreach (var keyword in SyntaxFacts.CodeKeywords
                                                   .Where(k => !SyntaxFacts.IsHiddenKeyword(k))
                                                   .OrderBy(k => k)) {

                    completionItems.Add(CreateKeywordCompletion(keyword));

                }

                return CreateCompletionContextTask(completionItems);
            }

            return CreateEmptyCompletionContextTask();
        }

        bool ShouldProvideCompletions(SnapshotPoint triggerLocation, out SnapshotSpan applicableToSpan) {
           
            var line                       = triggerLocation.GetContainingLine();
            var start                      = line.GetStartOfIdentifier(triggerLocation);
            var previousNonWhitespacePoint = line.GetPreviousNonWhitespace(start);
            var previousNonWhitespace      = previousNonWhitespacePoint?.GetChar();

            applicableToSpan = default;

            if (previousNonWhitespace == SyntaxFacts.OpenBracket) {
                applicableToSpan = new SnapshotSpan(start, triggerLocation);
                return true;
            }

            return false;
        }

    }

}