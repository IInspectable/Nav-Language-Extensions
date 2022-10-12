#region Using Directives

using JetBrains.Annotations;

using Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion.Data;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Completion;

static class CompletionFilters {

    public static CompletionFilter Keywords         = new("Keywords"         , "K", CompletionImages.Keyword);
    public static CompletionFilter Folders          = new("Folders"          , "D", CompletionImages.Folder);
    public static CompletionFilter Files            = new("Files"            , "F", CompletionImages.File);
    public static CompletionFilter Choices          = new("Choices"          , "C", CompletionImages.Choice);
    public static CompletionFilter GuiNodes         = new("Views and Dialogs", "V", CompletionImages.GuiNode);
    public static CompletionFilter ConnectionPoints = new("Connection Points", "P", CompletionImages.ConnectionPoint);
    public static CompletionFilter Tasks            = new("Tasks"            , "T", CompletionImages.Task);

    [CanBeNull]
    public static CompletionFilter TryGetFromSymbol(ISymbol symbol) {
        switch (symbol) {
            case IInitNodeSymbol:
            case IExitNodeSymbol:
            case IEndNodeSymbol:    return ConnectionPoints;
            case IChoiceNodeSymbol: return Choices;
            case IGuiNodeSymbol:    return GuiNodes;
            case ITaskNodeSymbol:   return Tasks;
        }

        return null;
    }

}