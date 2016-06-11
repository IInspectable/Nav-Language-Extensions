#region Using Directives

using System;
using System.ComponentModel.Composition;

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.Text.Tagging;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.CSharp.GoToNav {

    [Export(typeof(ITaggerProvider))]
    [ContentType("csharp")]
    [TagType(typeof(GoToNavTag))]
    sealed class GoToNavTaggerProvider : ITaggerProvider {

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag {
            if(buffer == null)
                throw new ArgumentNullException(nameof(buffer));

            return buffer.Properties.GetOrCreateSingletonProperty(
                () => new GoToNavTagger(buffer)) as ITagger<T>;
        }
    }
}
