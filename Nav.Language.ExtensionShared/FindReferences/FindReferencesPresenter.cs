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
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.FindAllReferences;
using Microsoft.VisualStudio.Text.Classification;

using Pharmatechnik.Nav.Language.Extension.Classification;
using Pharmatechnik.Nav.Language.Extension.Common;
using Pharmatechnik.Nav.Language.Extension.HighlightReferences;
using Pharmatechnik.Nav.Language.Extension.Utilities;
using Pharmatechnik.Nav.Language.Text;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.FindReferences {

    [Export(typeof(FindReferencesPresenter))]
    class FindReferencesPresenter {

        private readonly ProjectService                  _projectService;
        readonly         IEditorFormatMapService         _editorFormatMapService;
        readonly         IClassificationFormatMapService _classificationFormatMapService;
        readonly         IFindAllReferencesService       _vsFindAllReferencesService;

        readonly ImmutableDictionary<TextClassification, IClassificationType> _classificationMap;

        [ImportingConstructor]
        public FindReferencesPresenter(SVsServiceProvider serviceProvider,
                                       ProjectService projectService,
                                       IEditorFormatMapService editorFormatMapService,
                                       IClassificationFormatMapService classificationFormatMapService,
                                       IClassificationTypeRegistryService classificationTypeRegistryService) {

            _projectService                 = projectService;
            _editorFormatMapService         = editorFormatMapService;
            _classificationFormatMapService = classificationFormatMapService;
            _classificationMap              = ClassificationTypeDefinitions.GetSyntaxTokenClassificationMap(classificationTypeRegistryService);
            _vsFindAllReferencesService     = (IFindAllReferencesService) serviceProvider.GetService(typeof(SVsFindAllReferences));
            Assumes.Present(_vsFindAllReferencesService);
        }

        IClassificationFormatMap FormatMap    => _classificationFormatMapService.GetClassificationFormatMap("tooltip");
        IEditorFormatMap         EditorFormat => _editorFormatMapService.GetEditorFormatMap("text");

        Brush HighlightBackgroundBrush {
            get {
                var properties     = EditorFormat.GetProperties(MarkerFormatDefinitionNames.ReferenceHighlight);
                var highlightBrush = properties["Background"] as Brush;

                return highlightBrush;
            }
        }

        Brush ToolWindowBackgroundBrush => (Brush) Application.Current.Resources[EnvironmentColors.ToolWindowBackgroundBrushKey];

        public FindReferencesContext StartSearch() {

            ThreadHelper.ThrowIfNotOnUIThread();

            var window        = _vsFindAllReferencesService.StartSearch("Find References");
            var projectMapper = _projectService.GetProjectMapper();
            var context       = new FindReferencesContext(this, window, projectMapper);

            return context;
        }

        public void HighlightBackground(Run run) {

            var highlightBrush = HighlightBackgroundBrush;

            if (highlightBrush == null) {
                return;
            }

            run.SetValue(
                TextElement.BackgroundProperty,
                HighlightBackgroundBrush);

        }

        public void SetBold(Run run) {
            run.SetValue(TextElement.FontWeightProperty, FontWeights.Bold);

        }

        public ToolTip CreateToolTip(object content) {

            return new ToolTip {
                Background = ToolWindowBackgroundBrush,
                Content    = content
            };
        }

        public TextBlock ToTextBlock(IEnumerable<ClassifiedText> parts, Action<Run, ClassifiedText, int> runAction = null, bool consolidateWhitespace = true) {

            var textBlock = new TextBlock {
                TextWrapping = TextWrapping.NoWrap,
                TextTrimming = TextTrimming.CharacterEllipsis
            };

            textBlock.SetDefaultTextProperties(FormatMap);

            var inlines = ToInlines(parts, runAction, consolidateWhitespace);
            textBlock.Inlines.AddRange(inlines);

            return textBlock;
        }

        public IEnumerable<Inline> ToInlines(IEnumerable<ClassifiedText> parts, Action<Run, ClassifiedText, int> runAction = null, bool consolidateWhitespace = true) {

            var position = 0;
            foreach (var part in parts) {

                var inline = ToInline(part, consolidateWhitespace, isLineStart: position == 0);

                runAction?.Invoke(inline, part, position);

                position += part.Text.Length;

                yield return inline;
            }

        }

        Run ToInline(ClassifiedText classifiedText, bool consolidateWhitespace, bool isLineStart) {
            return ToInline(classifiedText.Text, classifiedText.Classification, consolidateWhitespace, isLineStart);
        }

        Run ToInline(string text, TextClassification classification, bool consolidateWhitespace, bool isLineStart) {

            // Es nervt in der Vorschau, wenn Tabluatoren den Text unnötig in die Länge ziehen. Deswegen dampfen wir
            // Whitepaces auf ein Leerzeichen respektive NL ein.
            if (consolidateWhitespace &&
                classification == TextClassification.Whitespace) {

                var ws = " ";
                if (isLineStart) {
                    ws = String.Empty;
                }
                // NL dürfen wir nicht einfach wegwerfen.
                text = text.GetNewLineCharCount() == 0 ? ws : Environment.NewLine;
            }

            var run = new Run(text);

            _classificationMap.TryGetValue(classification, out var ct);

            if (ct != null) {
                var props = FormatMap.GetTextProperties(ct);
                run.SetTextProperties(props);

            }

            return run;
        }

    }

}