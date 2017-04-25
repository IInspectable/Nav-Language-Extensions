#region Using Directives

using System.ComponentModel.Composition;

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Language.Intellisense;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.CodeFixes {

    [Export(typeof(ISuggestedActionsSourceProvider))]
    [Name(nameof(CodeFixSuggestedActionsSourceProvider))]
    [ContentType(NavLanguageContentDefinitions.ContentType)]
    internal class CodeFixSuggestedActionsSourceProvider : ISuggestedActionsSourceProvider {

        readonly ICodeFixActionProviderService _codeFixActionProviderService;

        [ImportingConstructor]
        public CodeFixSuggestedActionsSourceProvider(ICodeFixActionProviderService codeFixActionProviderService) {
            _codeFixActionProviderService = codeFixActionProviderService;
        }
        
        public ISuggestedActionsSource CreateSuggestedActionsSource(ITextView textView, ITextBuffer textBuffer) {
            if (textBuffer == null && textView == null) {
                return null;
            }
            // TODO nur einzelne Textbuffer unterst�tzen?
            return new CodeFixSuggestedActionsSource(textBuffer, _codeFixActionProviderService);
        }
    }
}