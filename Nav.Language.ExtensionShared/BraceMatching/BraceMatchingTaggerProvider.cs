#region Using Directives

using System.ComponentModel.Composition;

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;

using Microsoft.VisualStudio.Utilities;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.BraceMatching {
    
    [Export(typeof (IViewTaggerProvider))]
    [ContentType(NavLanguageContentDefinitions.ContentType)]
    [TagType(typeof (TextMarkerTag))]
    class BraceMatchingTaggerProvider : IViewTaggerProvider {

        public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer) where T : ITag {

            if (textView == null) {
                return null;
            }

            // Provide highlighting only on the top-level buffer
            if (textView.TextBuffer != buffer) {
                return null;
            }

            return new BraceMatchingTagger(textView, buffer) as ITagger<T>;
        }
    }
}