#region Using Directives

using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Collections.Generic;
using System.ComponentModel.Composition;

using Microsoft.VisualStudio.Text.Classification;

using Pharmatechnik.Nav.Language.CodeGen;
using Pharmatechnik.Nav.Language.Extension.Classification;
using Pharmatechnik.Nav.Language.Extension.Common;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.QuickInfo {

    [Export]
    sealed class SyntaxQuickinfoBuilderService {

        readonly IClassificationFormatMapService _classificationFormatMapService;
        readonly Dictionary<SyntaxTokenClassification, IClassificationType> _classificationMap;

        [ImportingConstructor]
        public SyntaxQuickinfoBuilderService(IClassificationFormatMapService classificationFormatMapService,
                                             IClassificationTypeRegistryService classificationTypeRegistryService) {

            _classificationFormatMapService = classificationFormatMapService;
            _classificationMap              = ClassificationTypeDefinitions.GetSyntaxTokenClassificationMap(classificationTypeRegistryService);

        }

        public TextBlock ToTextBlock(string text, SyntaxTokenClassification classification) {

            var textBlock = new TextBlock { TextWrapping = TextWrapping.Wrap };

            var formatMap = _classificationFormatMapService.GetClassificationFormatMap("tooltip");
            textBlock.SetDefaultTextProperties(formatMap);

            var run = ToRun(text, classification, formatMap);

            textBlock.Inlines.Add(run);

            return textBlock;
        }

        public TextBlock ToTextBlock(SyntaxTree syntaxTree) {

            var textBlock = new TextBlock { TextWrapping = TextWrapping.Wrap };
            var formatMap = _classificationFormatMapService.GetClassificationFormatMap("tooltip");

            textBlock.SetDefaultTextProperties(formatMap);

            foreach(var token in syntaxTree.Tokens) {

                var run = ToRun(token.ToString(), token.Classification, formatMap);
                
                textBlock.Inlines.Add(run);
            }

            return textBlock;
        }

        public TextBlock ToTextBlock(SignalTriggerCodeInfo codeInfo) {

            var textBlock = new TextBlock { TextWrapping = TextWrapping.Wrap };
            var formatMap = _classificationFormatMapService.GetClassificationFormatMap("tooltip");

            textBlock.SetDefaultTextProperties(formatMap);

            //var nsRun = ToRun(codeModel.WflNamespace+".", SyntaxTokenClassification.Identifier, formatMap);
            //textBlock.Inlines.Add(nsRun);

            var typeRun = ToRun(codeInfo.Task.WfsTypeName, SyntaxTokenClassification.TaskName, formatMap);
            textBlock.Inlines.Add(typeRun);

            var methodRun = ToRun("."+ codeInfo.TriggerLogicMethodName + "()", SyntaxTokenClassification.Identifier, formatMap);
            textBlock.Inlines.Add(methodRun);

            return textBlock;
        }

        Run ToRun(string text, SyntaxTokenClassification classification, IClassificationFormatMap formatMap) {
            var run = new Run(text);

            IClassificationType ct;
            _classificationMap.TryGetValue(classification, out ct);
            if (ct != null) {
                var props = formatMap.GetTextProperties(ct);
                run.SetTextProperties(props);
            }

            return run;
        }
    }
}