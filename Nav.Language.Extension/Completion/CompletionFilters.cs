#region Using Directives

using JetBrains.Annotations;

using Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion.Data;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Completion {

    static class CompletionFilters {

        public static CompletionFilter Keywords         = new CompletionFilter("Keywords",          "K", CompletionImages.Keyword);
        public static CompletionFilter Folders          = new CompletionFilter("Folders",           "D", CompletionImages.Folder);
        public static CompletionFilter Files            = new CompletionFilter("Files",             "F", CompletionImages.File);
        public static CompletionFilter Choices          = new CompletionFilter("Choices",           "C", CompletionImages.Choice);
        public static CompletionFilter GuiNodes         = new CompletionFilter("Views and Dialogs", "V", CompletionImages.GuiNode);
        public static CompletionFilter ConnectionPoints = new CompletionFilter("Connection Points", "P", CompletionImages.ConnectionPoint);
        public static CompletionFilter Tasks            = new CompletionFilter("Tasks",             "T", CompletionImages.Task);

        [CanBeNull]
        public static CompletionFilter TryGetFromSymbol(ISymbol symbol) {
            switch (symbol) {
                case IInitNodeSymbol _:
                case IExitNodeSymbol _:
                case IEndNodeSymbol _: return ConnectionPoints;
                case IChoiceNodeSymbol _: return Choices;
                case IGuiNodeSymbol _:    return GuiNodes;
                case ITaskNodeSymbol _:   return Tasks;
            }

            return null;
        }

    }

}