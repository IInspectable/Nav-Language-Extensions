#region Using Directives

using Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion.Data;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Completion3 {

    static class CompletionFilters {

        public static CompletionFilter Keyword = new CompletionFilter("Keyword", "K", CompletionImages.Keyword);

    }

}