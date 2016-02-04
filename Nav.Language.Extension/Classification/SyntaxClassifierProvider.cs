#region Using Directives

using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Classification {

    [Export(typeof(IClassifierProvider))]
    [ContentType(NavLanguageContentDefinitions.ContentType)] 
    sealed class SyntaxClassifierProvider : IClassifierProvider {

        readonly IClassificationTypeRegistryService _classificationTypeRegistryService;

        [ImportingConstructor]
        public SyntaxClassifierProvider(IClassificationTypeRegistryService classificationTypeRegistryService) {
            _classificationTypeRegistryService = classificationTypeRegistryService;
        }

        public IClassifier GetClassifier(ITextBuffer buffer) {
            return SyntaxClassifier.GetOrCreateSingelton(_classificationTypeRegistryService, buffer);
        }
    }
}