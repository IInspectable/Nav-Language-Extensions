#region Using Directives

using System.Linq;
using System.Collections.Generic;

using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.Text.Editor;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Common {

    static class TextViewExtensions {

        public static ISet<IContentType> GetContentTypes(this ITextView textView) {
            return new HashSet<IContentType>(
                textView.BufferGraph.GetTextBuffers(_ => true).Select(b => b.ContentType));
        }
    }
}
