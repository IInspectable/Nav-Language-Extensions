#region Using Directives

using System.Collections.Generic;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.QuickInfo {

   //[Export(typeof (IIntellisenseControllerProvider))]
   //[Name("ToolTip Debug Info Controller")]
   //[ContentType(NavLanguageContentDefinitions.ContentType)]
    sealed class IntellisenseControllerProvider: IIntellisenseControllerProvider {

        [ImportingConstructor]
        public IntellisenseControllerProvider(IQuickInfoBroker quickInfoBroker) {
            QuickInfoBroker = quickInfoBroker;
        }

        public IQuickInfoBroker QuickInfoBroker { get; }

        public IIntellisenseController TryCreateIntellisenseController(ITextView textView, IList<ITextBuffer> subjectBuffers) {
            return new IntellisenseController(textView, subjectBuffers, this);
        }
    }
}