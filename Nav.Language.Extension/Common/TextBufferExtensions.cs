#region Using Directives

using JetBrains.Annotations;
using Microsoft.VisualStudio.Text;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Common {

    static class TextBufferExtensions {

        [CanBeNull]
        public static ITextDocument GetTextDocument(this ITextBuffer textBuffer) {
            ITextDocument textDoc;
            var rc = textBuffer.Properties.TryGetProperty(typeof(ITextDocument), out textDoc);
            return rc ? textDoc : null;
        }
    }
}