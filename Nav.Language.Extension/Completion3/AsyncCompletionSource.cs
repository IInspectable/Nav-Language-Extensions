#region Using Directives

using System.Collections.Immutable;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.VisualStudio.Core.Imaging;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion;
using Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion.Data;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Adornments;

using Pharmatechnik.Nav.Language.Extension.Completion2;
using Pharmatechnik.Nav.Language.Extension.Images;
using Pharmatechnik.Nav.Language.Extension.QuickInfo;
using Pharmatechnik.Nav.Utilities.IO;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Completion3 {

    abstract class AsyncCompletionSource: IAsyncCompletionSource {

        protected AsyncCompletionSource(QuickinfoBuilderService quickinfoBuilderService) {
            QuickinfoBuilderService = quickinfoBuilderService;

        }

        public QuickinfoBuilderService QuickinfoBuilderService { get; }

        public abstract bool TryGetApplicableToSpan(char typedChar, SnapshotPoint triggerLocation, out SnapshotSpan applicableToSpan, CancellationToken token);
        public abstract Task<CompletionContext> GetCompletionContextAsync(InitialTrigger trigger, SnapshotPoint triggerLocation, SnapshotSpan applicableToSpan, CancellationToken token);

        public virtual Task<object> GetDescriptionAsync(CompletionItem item, CancellationToken token) {
            if (item.Properties.TryGetProperty<ISymbol>(CompletionElementProvider.SymbolPropertyName, out var symbol)) {
                return Task.FromResult((object) QuickinfoBuilderService.BuildSymbolQuickInfoContent(symbol));
            }

            if (item.Properties.TryGetProperty<string>(CompletionElementProvider.KeywordPropertyName, out var keyword)) {
                return Task.FromResult((object) QuickinfoBuilderService.BuildKeywordQuickInfoContent(keyword));
            }

            return Task.FromResult((object) item.DisplayText);
        }

        protected static Task<CompletionContext> CreateCompletionContext(ImmutableArray<CompletionItem>.Builder itemsBuilder) {
            var context = new CompletionContext(itemsBuilder.ToImmutable());
            return Task.FromResult(context);
        }

        protected static Task<CompletionContext> CreateEmptyCompletionContext() {
            return Task.FromResult(new CompletionContext(ImmutableArray<CompletionItem>.Empty));
        }

        protected CompletionItem CreateSymbolCompletion(ISymbol symbol, string description) {

            var imageMoniker = ImageMonikers.FromSymbol(symbol);
            var imageElement = new ImageElement(imageMoniker.ToImageId());

            var completionItem = new CompletionItem(displayText: symbol.Name,
                                                    source: this,
                                                    icon: imageElement);

            completionItem.Properties.AddProperty(CompletionElementProvider.SymbolPropertyName, symbol);

            return completionItem;
        }

        protected CompletionItem CreateKeywordCompletion(string keyword) {

            var imageMoniker = KnownMonikers.IntellisenseKeyword;
            var imageElement = new ImageElement(imageMoniker.ToImageId());

            var completionItem = new CompletionItem(displayText: keyword,
                                                    source: this,
                                                    icon: imageElement);

            completionItem.Properties.AddProperty(CompletionElementProvider.KeywordPropertyName, keyword);

            return completionItem;
        }

        protected CompletionItem CreateFileNameCompletion(DirectoryInfo directory, FileInfo file) {

            var directoryName = directory.FullName + Path.DirectorySeparatorChar;
            var relativePath  = PathHelper.GetRelativePath(fromPath: directoryName, toPath: file.FullName);
            var displayPath   = CompactPath(relativePath, 50);
            var imageMoniker  = ImageMonikers.Include;
            var imageElement  = new ImageElement(imageMoniker.ToImageId());

            var completionItem = new CompletionItem(displayText: displayPath,
                                                    source: this,
                                                    icon: imageElement,
                                                    filters: ImmutableArray<CompletionFilter>.Empty,
                                                    suffix: "",
                                                    insertText: relativePath,
                                                    sortText: file.Name,
                                                    filterText: file.Name,
                                                    attributeIcons: ImmutableArray<ImageElement>.Empty
            );

            return completionItem;
        }

        [DllImport("shlwapi.dll", CharSet = CharSet.Auto)]
        static extern bool PathCompactPathEx([Out] StringBuilder pszOut, string szPath, int cchMax, int dwFlags);

        static string CompactPath(string longPathName, int wantedLength) {
            // NOTE: You need to create the builder with the required capacity before calling function.
            // See http://msdn.microsoft.com/en-us/library/aa446536.aspx
            StringBuilder sb = new StringBuilder(wantedLength + 1);
            PathCompactPathEx(sb, longPathName, wantedLength + 1, 0);
            return sb.ToString();
        }

    }

}