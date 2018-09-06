#region Using Directives

using Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion.Data;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Completion {

    static class CompletionFilters {

        public static CompletionFilter Keyword = new CompletionFilter("Keyword", "K", CompletionImages.Keyword);

    }

}