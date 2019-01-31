﻿#region Using Directives

using System.Collections.Immutable;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using JetBrains.Annotations;

using Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion;
using Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion.Data;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Adornments;

using Pharmatechnik.Nav.Language.Extension.QuickInfo;
using Pharmatechnik.Nav.Utilities.IO;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Completion {

    abstract class AsyncCompletionSource: IAsyncCompletionSource {

        protected AsyncCompletionSource(QuickinfoBuilderService quickinfoBuilderService) {
            QuickinfoBuilderService = quickinfoBuilderService;

        }

        public QuickinfoBuilderService QuickinfoBuilderService { get; }

        public abstract CompletionStartData InitializeCompletion(CompletionTrigger trigger, SnapshotPoint triggerLocation, CancellationToken token);

        public abstract Task<CompletionContext> GetCompletionContextAsync(IAsyncCompletionSession session, CompletionTrigger trigger, SnapshotPoint triggerLocation, SnapshotSpan applicableToSpan, CancellationToken token);

        public virtual Task<object> GetDescriptionAsync(IAsyncCompletionSession session, CompletionItem item, CancellationToken token) {

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

        protected static Task<CompletionContext> CreateCompletionContextTaskAsync(ImmutableArray<CompletionItem>.Builder itemsBuilder,
                                                                                  InitialSelectionHint initialSelectionHint = InitialSelectionHint.SoftSelection) {
            return Task.FromResult(CreateCompletionContext(itemsBuilder, initialSelectionHint));
        }

        protected static CompletionContext CreateCompletionContext(ImmutableArray<CompletionItem>.Builder itemsBuilder,
                                                                   InitialSelectionHint initialSelectionHint = InitialSelectionHint.SoftSelection) {
            return new CompletionContext(itemsBuilder.ToImmutable(), null, initialSelectionHint);
        }

        protected static Task<CompletionContext> CreateEmptyCompletionContextTaskAsync() {
            return Task.FromResult(CreateEmptyCompletionContext());
        }

        protected static CompletionContext CreateEmptyCompletionContext() {
            return new CompletionContext(ImmutableArray<CompletionItem>.Empty);
        }

        protected CompletionItem CreateSymbolCompletion(ISymbol symbol, string description) {

            var filter = CompletionFilters.TryGetFromSymbol(symbol);

            var filters = filter != null ? ImmutableArray.Create(filter) : ImmutableArray<CompletionFilter>.Empty;

            var completionItem = new CompletionItem(displayText: symbol.Name,
                                                    source     : this,
                                                    icon       : CompletionImages.FromSymbol(symbol),
                                                    filters    : filters);

            completionItem.Properties.AddProperty(SymbolPropertyName, symbol);

            return completionItem;
        }

        protected CompletionItem CreateKeywordCompletion(string keyword) {

            var completionItem = new CompletionItem(displayText: keyword,
                                                    source     : this,
                                                    icon       : CompletionImages.Keyword,
                                                    filters    : ImmutableArray.Create(CompletionFilters.Keywords)
            );

            completionItem.Properties.AddProperty(KeywordPropertyName, keyword);

            return completionItem;
        }

        protected CompletionItem CreateDirectoryInfoCompletion(DirectoryInfo directory,
                                                               DirectoryInfo dir,
                                                               [CanBeNull] string displayText = null,
                                                               [CanBeNull] ImageElement icon = null,
                                                               [CanBeNull] ITrackingSpan replacementSpan = null) {

            var directoryName = directory.FullName + Path.DirectorySeparatorChar;
            var relativePath  = PathHelper.GetRelativePath(fromPath: directoryName, toPath: dir.FullName + Path.DirectorySeparatorChar);

            displayText = displayText ?? dir.Name;

            var completionItem = new CompletionItem(displayText   : displayText,
                                                    source        : this,
                                                    icon          : icon ?? CompletionImages.Folder,
                                                    filters       : ImmutableArray.Create(CompletionFilters.Folders),
                                                    suffix        : "",
                                                    insertText    : relativePath,
                                                    sortText      : $"__{displayText}",
                                                    filterText    : displayText,
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

            var completionItem = new CompletionItem(displayText   : displayText,
                                                    source        : this,
                                                    icon          : CompletionImages.NavFile,
                                                    filters       : ImmutableArray.Create(CompletionFilters.Files),
                                                    suffix        : "",
                                                    insertText    : relativePath,
                                                    sortText      : $"_{displayText}",
                                                    filterText    : file.Name,
                                                    attributeIcons: ImmutableArray<ImageElement>.Empty);

            completionItem.Properties.AddProperty(NavFileInfoPropertyName, file);
            if (replacementSpan != null) {
                completionItem.Properties.AddProperty(ReplacementTrackingSpanProperty, replacementSpan);
            }

            return completionItem;
        }

        // ReSharper disable InconsistentNaming
        public static string SymbolPropertyName        => nameof(SymbolPropertyName);
        public static string KeywordPropertyName       => nameof(KeywordPropertyName);
        public static string DirectoryInfoPropertyName => nameof(DirectoryInfoPropertyName);
        public static string NavFileInfoPropertyName   => nameof(NavFileInfoPropertyName);

        public static string ReplacementTrackingSpanProperty => nameof(ReplacementTrackingSpanProperty);
        // ReSharper restore InconsistentNaming

        protected static CodeGenerationUnit GetCodeGenerationUnit(SnapshotPoint triggerLocation) {

            var semanticModelService = SemanticModelService.GetOrCreateSingelton(triggerLocation.Snapshot.TextBuffer);

            var generationUnitAndSnapshot = semanticModelService.UpdateSynchronously();
            var codeGenerationUnit        = generationUnitAndSnapshot.CodeGenerationUnit;

            return codeGenerationUnit;
        }

    }

}