#region Using Directives

using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion.Data;
using Microsoft.VisualStudio.Text;

using Pharmatechnik.Nav.Language.Extension.Completion2;
using Pharmatechnik.Nav.Language.Extension.QuickInfo;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Completion3 {

    class CodeCompletionSource: AsyncCompletionSource {

        public CodeCompletionSource(QuickinfoBuilderService quickinfoBuilderService)
            : base(quickinfoBuilderService) {

        }

        public override bool TryGetApplicableToSpan(char typedChar, SnapshotPoint triggerLocation, out SnapshotSpan applicableToSpan, CancellationToken token) {

            applicableToSpan = default;

            var line                       = triggerLocation.GetContainingLine();
            var start                      = line.GetStartOfIdentifier(triggerLocation);
            var previousNonWhitespacePoint = line.GetPreviousNonWhitespace(start);
            var previousNonWhitespace      = previousNonWhitespacePoint?.GetChar();

            if (previousNonWhitespace == SyntaxFacts.OpenBracket) {
                applicableToSpan = new SnapshotSpan(start, triggerLocation);
                return true;
            }

            return false;
        }

        public override Task<CompletionContext> GetCompletionContextAsync(InitialTrigger trigger, SnapshotPoint triggerLocation, SnapshotSpan applicableToSpan, CancellationToken token) {
            var line                       = triggerLocation.GetContainingLine();
            var start                      = line.GetStartOfIdentifier(triggerLocation);
            var previousNonWhitespacePoint = line.GetPreviousNonWhitespace(start);
            var previousNonWhitespace      = previousNonWhitespacePoint?.GetChar();

            if (previousNonWhitespace == SyntaxFacts.OpenBracket) {
                var completionItems = ImmutableArray.CreateBuilder<CompletionItem>();

                foreach (var keyword in SyntaxFacts.CodeKeywords) {
                    completionItems.Add(CreateKeywordCompletion(keyword));
                }

                return CreateCompletionContext(completionItems);
            }

            return CreateEmptyCompletionContext();
        }

    }

}