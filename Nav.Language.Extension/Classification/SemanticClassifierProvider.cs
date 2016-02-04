#region Using Directives

using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Classification {

    [Export(typeof(IClassifierProvider))]
    [ContentType(NavLanguageContentDefinitions.ContentType)]
    sealed class SemanticClassifierProvider : IClassifierProvider {

        readonly IClassificationTypeRegistryService _classificationTypeRegistryService;

        [ImportingConstructor]
        public SemanticClassifierProvider(IClassificationTypeRegistryService classificationTypeRegistryService) {
            _classificationTypeRegistryService = classificationTypeRegistryService;
        }

        public IClassifier GetClassifier(ITextBuffer buffer) {
            return SemanticClassifier.GetOrCreateSingelton(_classificationTypeRegistryService, buffer);
        }
    }
}