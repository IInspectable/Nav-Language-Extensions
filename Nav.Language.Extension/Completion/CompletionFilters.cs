#region Using Directives

using JetBrains.Annotations;

using Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion.Data;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Completion {

    static class CompletionFilters {

        public static CompletionFilter Keyword         = new CompletionFilter("Keyword",          "K", CompletionImages.Keyword);
        public static CompletionFilter Folder          = new CompletionFilter("Folder",           "D", CompletionImages.Folder);
        public static CompletionFilter File            = new CompletionFilter("File",             "F", CompletionImages.File);
        public static CompletionFilter Choice          = new CompletionFilter("Choice",           "C", CompletionImages.Choice);
        public static CompletionFilter GuiNode         = new CompletionFilter("View",             "V", CompletionImages.GuiNode);
        public static CompletionFilter ConnectionPoint = new CompletionFilter("Connection Point", "P", CompletionImages.ConnectionPoint);
        public static CompletionFilter Task            = new CompletionFilter("Task",             "T", CompletionImages.Task);

        [CanBeNull]
        public static CompletionFilter TryGetFromSymbol(ISymbol symbol) {
            switch (symbol) {
                case IInitNodeSymbol _:
                case IExitNodeSymbol _:
                case IEndNodeSymbol _: return ConnectionPoint;
                case IChoiceNodeSymbol _: return Choice;
                case IGuiNodeSymbol _:    return GuiNode;
                case ITaskNodeSymbol _:   return Task;
            }

            return null;
        }

    }

}