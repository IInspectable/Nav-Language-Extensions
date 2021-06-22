#region Using Directives

using System.Windows.Threading;

using JetBrains.Annotations;

using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.Text;

using Pharmatechnik.Nav.Utilities.Logging;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Common {

    static class TextBufferExtensions {

        static readonly Logger Logger = Logger.Create(typeof(TextBufferExtensions));

        [CanBeNull]
        public static ITextDocument GetTextDocument(this ITextBuffer textBuffer) {

            textBuffer.Properties.TryGetProperty(typeof(ITextDocument), out ITextDocument textDoc);

            if (textDoc == null) {
                Logger.Warn($"{nameof(GetTextDocument)}: There's no ITextDocument for the {nameof(ITextBuffer)}");
                return null;
            }

            return textDoc;
        }

        [CanBeNull]
        public static Project GetContainingProject(this ITextBuffer textBuffer) {

            Dispatcher.CurrentDispatcher.VerifyAccess();

            var filePath = textBuffer.GetTextDocument()?.FilePath;

            return NavLanguagePackage.GetContainingProject(filePath);
        }

    }

}