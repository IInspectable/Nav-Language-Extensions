#region Using Directives

using System.Collections.Immutable;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using JetBrains.Annotations;

using Microsoft.VisualStudio.Core.Imaging;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion;
using Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion.Data;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Adornments;

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
            if (item.Properties.TryGetProperty<ISymbol>(SymbolPropertyName, out var symbol)) {
                return Task.FromResult((object) QuickinfoBuilderService.BuildSymbolQuickInfoContent(symbol));
            }

            if (item.Properties.TryGetProperty<string>(KeywordPropertyName, out var keyword)) {
                return Task.FromResult((object) QuickinfoBuilderService.BuildKeywordQuickInfoContent(keyword));
            }

            if (item.Properties.TryGetProperty<DirectoryInfo>(DirectoryInfoPropertyName, out var dirInfo)) {
                return Task.FromResult((object) QuickinfoBuilderService.BuildDirectoryInfoQuickInfoContent(dirInfo));
            }

            if (item.Properties.TryGetProperty<FileInfo>(NavFileInfoPropertyName, out var fileInfo)) {
                return Task.FromResult((object) QuickinfoBuilderService.BuildNavFileInfoQuickInfoContent(fileInfo));
            }

            return Task.FromResult((object) item.DisplayText);
        }
            
        protected static Task<CompletionContext> CreateCompletionContext(ImmutableArray<CompletionItem>.Builder itemsBuilder, 
                                                                         InitialSelectionHint initialSelectionHint = InitialSelectionHint.SoftSelection) {
            var context = new CompletionContext(itemsBuilder.ToImmutable(), null, initialSelectionHint );
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

            completionItem.Properties.AddProperty(SymbolPropertyName, symbol);

            return completionItem;
        }

        private ImageElement _keywordImage;

        protected ImageElement KeywordImage {
            get {
                if (_keywordFilter == null) {
                    _keywordImage = new ImageElement(KnownMonikers.IntellisenseKeyword.ToImageId());
                }

                return _keywordImage;
            }
        }

        private ImageElement _folderImage;

        private ImageElement FolderImage {
            get {

                if (_folderImage == null) {
                    _folderImage = new ImageElement(ImageMonikers.FolderClosed.ToImageId());
                }

                return _folderImage;
            }
        }

        private ImageElement _navFileImage;

        private ImageElement NavFileImage {
            get {

                if (_navFileImage == null) {
                    _navFileImage = new ImageElement(ImageMonikers.Include.ToImageId());
                }

                return _navFileImage;
            }
        }

        CompletionFilter _keywordFilter;

        protected CompletionFilter KeywordFilter {
            get {
                if (_keywordFilter == null) {

                    _keywordFilter = new CompletionFilter("Keyword", "K", KeywordImage);
                }

                return _keywordFilter;
            }
        }

        protected CompletionItem CreateKeywordCompletion(string keyword) {

            var completionItem = new CompletionItem(displayText: keyword,
                                                    source: this,
                                                    icon: KeywordImage,
                                                    filters: new[] {KeywordFilter}.ToImmutableArray()
            );

            completionItem.Properties.AddProperty(KeywordPropertyName, keyword);

            return completionItem;
        }

        protected CompletionItem CreateDirectoryInfoCompletion(DirectoryInfo directory,
                                                               DirectoryInfo dir,
                                                               [CanBeNull] string displayText = null,
                                                               [CanBeNull] ITrackingSpan replacementSpan = null) {

            var directoryName = directory.FullName + Path.DirectorySeparatorChar;
            var relativePath  = PathHelper.GetRelativePath(fromPath: directoryName, toPath: dir.FullName + Path.DirectorySeparatorChar);

            displayText = displayText ?? dir.Name;

            var completionItem = new CompletionItem(displayText: displayText,
                                                    source: this,
                                                    icon: FolderImage,
                                                    filters: ImmutableArray<CompletionFilter>.Empty,
                                                    suffix: "",
                                                    insertText: relativePath,
                                                    sortText: $"__{displayText}",
                                                    filterText: displayText,
                                                    attributeIcons: ImmutableArray<ImageElement>.Empty);

            completionItem.Properties.AddProperty(DirectoryInfoPropertyName, dir);
            if (replacementSpan != null) {
                completionItem.Properties.AddProperty(ReplacementTrackingSpanProperty, replacementSpan);
            }

            return completionItem;
        }

        protected CompletionItem CreateFileInfoCompletion(DirectoryInfo directory,
                                                          FileInfo file,
                                                          [CanBeNull] string displayText = null,
                                                          [CanBeNull] ITrackingSpan replacementSpan = null) {

            var directoryName = directory.FullName + Path.DirectorySeparatorChar;
            var relativePath  = PathHelper.GetRelativePath(fromPath: directoryName, toPath: file.FullName);

            displayText = displayText ?? file.Name;

            var completionItem = new CompletionItem(displayText: displayText,
                                                    source: this,
                                                    icon: NavFileImage,
                                                    filters: ImmutableArray<CompletionFilter>.Empty,
                                                    suffix: "",
                                                    insertText: relativePath,
                                                    sortText: $"_{displayText}",
                                                    filterText: file.Name,
                                                    attributeIcons: ImmutableArray<ImageElement>.Empty);

            completionItem.Properties.AddProperty(NavFileInfoPropertyName, file);
            if (replacementSpan != null) {
                completionItem.Properties.AddProperty(ReplacementTrackingSpanProperty, replacementSpan);
            }

            return completionItem;
        }

        public static string SymbolPropertyName              => nameof(SymbolPropertyName);
        public static string KeywordPropertyName             => nameof(KeywordPropertyName);
        public static string DirectoryInfoPropertyName       => nameof(DirectoryInfoPropertyName);
        public static string NavFileInfoPropertyName         => nameof(NavFileInfoPropertyName);
        public static string ReplacementTrackingSpanProperty => nameof(ReplacementTrackingSpanProperty);

    }

}