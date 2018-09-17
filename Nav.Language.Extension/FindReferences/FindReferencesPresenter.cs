#region Using Directives

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

using Microsoft;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.FindAllReferences;
using Microsoft.VisualStudio.Text.Classification;

using Pharmatechnik.Nav.Language.Extension.Classification;
using Pharmatechnik.Nav.Language.Extension.Common;
using Pharmatechnik.Nav.Language.Extension.HighlightReferences;
using Pharmatechnik.Nav.Language.Text;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.FindReferences {

    [Export(typeof(FindReferencesPresenter))]
    class FindReferencesPresenter {

        readonly IEditorFormatMapService         _editorFormatMapService;
        readonly IClassificationFormatMapService _classificationFormatMapService;

        readonly IFindAllReferencesService _vsFindAllReferencesService;

        [ImportingConstructor]
        public FindReferencesPresenter(SVsServiceProvider serviceProvider,
                                       IEditorFormatMapService editorFormatMapService,
                                       IClassificationFormatMapService classificationFormatMapService,
                                       IClassificationTypeRegistryService classificationTypeRegistryService) {

            _editorFormatMapService         = editorFormatMapService;
            _classificationFormatMapService = classificationFormatMapService;
            ClassificationMap               = ClassificationTypeDefinitions.GetSyntaxTokenClassificationMap(classificationTypeRegistryService);
            _vsFindAllReferencesService     = (IFindAllReferencesService) serviceProvider.GetService(typeof(SVsFindAllReferences));
            Assumes.Present(_vsFindAllReferencesService);
        }

        private const string Title = "Find References";

        public FindReferencesContext StartSearch() {

            var window  = _vsFindAllReferencesService.StartSearch(Title);
            var context = new FindReferencesContext(this, window);

            return context;
        }

        public ImmutableDictionary<TextClassification, IClassificationType> ClassificationMap;

        public IClassificationFormatMap FormatMap    => _classificationFormatMapService.GetClassificationFormatMap("tooltip");
        public IEditorFormatMap         EditorFormat => _editorFormatMapService.GetEditorFormatMap("text");

        public Brush HighlightBackgroundBrush {
            get {
                var properties = EditorFormat
                   .GetProperties(MarkerFormatDefinitionNames.ReferenceHighlight);
                var highlightBrush = properties["Background"] as Brush;
                return highlightBrush;
            }
        }

        public TextBlock ToTextBlock(IEnumerable<ClassifiedText> parts) {

            var textBlock = new TextBlock {TextWrapping = TextWrapping.Wrap};

            textBlock.SetDefaultTextProperties(FormatMap);

            foreach (var part in parts) {
                var run = ToInline(part);
                textBlock.Inlines.Add(run);
            }

            return textBlock;
        }

        public IEnumerable<Inline> ToInlines(IEnumerable<ClassifiedText> parts, Action<Run, ClassifiedText, int> runAction = null) {

            var position = 0;
            foreach (var part in parts) {

                var inline = ToInline(part);

                runAction?.Invoke(inline, part, position);

                position += part.Text.Length;

                yield return inline;
            }

        }

        Run ToInline(ClassifiedText classifiedText) {
            return ToInline(classifiedText.Text, classifiedText.Classification);
        }

        Run ToInline(string text, TextClassification classification) {

            var run = new Run(text);

            ClassificationMap.TryGetValue(classification, out var ct);

            if (ct != null) {
                var props = FormatMap.GetTextProperties(ct);
                run.SetTextProperties(props);

            }

            return run;
        }

    }

}