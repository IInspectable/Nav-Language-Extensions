#region Using Directives

using System.ComponentModel.Composition;

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.GoToDefinition {

    [Export(typeof(ITaggerProvider))]
    [ContentType(NavLanguageContentDefinitions.ContentType)]
    [TagType(typeof(GoToDefinitionTag))]
    sealed class GoToDefinitionTaggerProvider : ITaggerProvider {
        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag {
            return GoToDefinitionTagger.GetOrCreateSingelton<T>(buffer);
        }
    }
}