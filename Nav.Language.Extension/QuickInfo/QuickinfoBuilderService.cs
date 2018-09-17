#region Using Directives

using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Collections.Immutable;
using System.ComponentModel.Composition;
using System.IO;

using JetBrains.Annotations;

using Microsoft.VisualStudio.Text.Classification;

using Pharmatechnik.Nav.Language.Extension.Classification;
using Pharmatechnik.Nav.Language.Extension.Common;
using Pharmatechnik.Nav.Language.Extension.Images;
using Pharmatechnik.Nav.Language.Text;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.QuickInfo {

    [Export]
    sealed partial class QuickinfoBuilderService {

        readonly IClassificationFormatMapService                              _classificationFormatMapService;
        readonly ImmutableDictionary<TextClassification, IClassificationType> _classificationMap;

        [ImportingConstructor]
        public QuickinfoBuilderService(IClassificationFormatMapService classificationFormatMapService,
                                       IClassificationTypeRegistryService classificationTypeRegistryService) {

            _classificationFormatMapService = classificationFormatMapService;
            _classificationMap              = ClassificationTypeDefinitions.GetSyntaxTokenClassificationMap(classificationTypeRegistryService);

        }

        public IClassificationFormatMap ClassificationFormatMap => _classificationFormatMapService.GetClassificationFormatMap("tooltip");

        public UIElement BuildSymbolQuickInfoContent(ISymbol source) {
            return SymbolQuickInfoVisitor.Build(source, this);
        }

        public UIElement BuildKeywordQuickInfoContent(string keyword) {
            var control = new SymbolQuickInfoControl {
                CrispImage  = {Moniker = ImageMonikers.Keyword},
                TextContent = {Content = $"keyword {keyword}"}
            };
            return control;
        }

        public UIElement BuildNavFileInfoQuickInfoContent(FileInfo fileInfo) {
            var control = new SymbolQuickInfoControl {
                CrispImage  = {Moniker = ImageMonikers.NavFile},
                TextContent = {Content = $"{fileInfo.FullName}"}
            };
            return control;
        }

        public UIElement BuildDirectoryInfoQuickInfoContent(DirectoryInfo dirInfo) {
            var control = new SymbolQuickInfoControl {
                CrispImage  = {Moniker = ImageMonikers.FolderClosed},
                TextContent = {Content = $"{dirInfo.FullName}"}
            };
            return control;
        }

        [CanBeNull]
        SymbolQuickInfoControl CreateDefaultSymbolQuickInfoControl(ISymbol symbol) {

            var imageMoniker = ImageMonikers.FromSymbol(symbol);
            var content      = ToTextBlock(symbol.ToDisplayParts());

            var control = new SymbolQuickInfoControl {
                CrispImage  = {Moniker = imageMoniker},
                TextContent = {Content = content}
            };

            return control;

        }

        [CanBeNull]
        TextBlock ToTextBlock(string text, TextClassification classification) {
            return ToTextBlock(new ClassifiedText(text, classification));
        }

        [CanBeNull]
        TextBlock ToTextBlock(params ClassifiedText[] parts) {
            return ToTextBlock(parts.ToImmutableArray());
        }

        [CanBeNull]
        TextBlock ToTextBlock(IReadOnlyCollection<ClassifiedText> parts) {

            if (parts.Count == 0) {
                return null;
            }

            var textBlock = new TextBlock {TextWrapping = TextWrapping.Wrap};

            textBlock.SetDefaultTextProperties(ClassificationFormatMap);

            foreach (var part in parts) {
                var inline = ToInline(part.Text, part.Classification, ClassificationFormatMap);
                textBlock.Inlines.Add(inline);
            }

            return textBlock;
        }

        Run ToInline(string text, TextClassification classification, IClassificationFormatMap formatMap) {
           
            var inline = new Run(text);

            _classificationMap.TryGetValue(classification, out var ct);
            if (ct != null) {
                var props = formatMap.GetTextProperties(ct);
                inline.SetTextProperties(props);
            }

            return inline;
        }

    }

}