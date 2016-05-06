#region Using Directives

using System.ComponentModel.Composition;

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

using Pharmatechnik.Nav.Language.Extension.Underlining;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Classification {

    [Export(typeof(IClassifierProvider))]
    [ContentType(NavLanguageContentDefinitions.ContentType)]
    sealed class UnderlineClassifierProvider : IClassifierProvider {

        readonly IClassificationTypeRegistryService _classificationTypeRegistryService;
        readonly IBufferTagAggregatorFactoryService _aggregatorFactory;

        [ImportingConstructor]
        public UnderlineClassifierProvider(IClassificationTypeRegistryService classificationTypeRegistryService,
                IBufferTagAggregatorFactoryService aggregatorFactory) {
            _classificationTypeRegistryService = classificationTypeRegistryService;
            _aggregatorFactory = aggregatorFactory;
        }

        public IClassifier GetClassifier(ITextBuffer buffer) {
            var underlineTagAggregator = _aggregatorFactory.CreateTagAggregator<UnderlineTag>(buffer);
            return UnderlineClassifier.GetOrCreateSingelton(_classificationTypeRegistryService, buffer, underlineTagAggregator);
        }
    }
}