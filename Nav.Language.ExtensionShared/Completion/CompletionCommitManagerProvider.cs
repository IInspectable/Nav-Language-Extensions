#region Using Directives

using System.Collections.Generic;
using System.ComponentModel.Composition;

using Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Completion; 

[Export(typeof(IAsyncCompletionCommitManagerProvider))]
[Name(nameof(CompletionCommitManagerProvider))]
[ContentType(NavLanguageContentDefinitions.ContentType)]
[TextViewRole(PredefinedTextViewRoles.Editable)]
class CompletionCommitManagerProvider: IAsyncCompletionCommitManagerProvider {

    readonly IDictionary<ITextView, IAsyncCompletionCommitManager> _cache = new Dictionary<ITextView, IAsyncCompletionCommitManager>();

    public IAsyncCompletionCommitManager GetOrCreate(ITextView textView) {

        if (_cache.TryGetValue(textView, out var itemSource)) {
            return itemSource;
        }

        var manager = new CompletionCommitManager();
        textView.Closed += (o, e) => _cache.Remove(textView); // clean up memory as files are closed
        _cache.Add(textView, manager);

        return manager;
    }

}