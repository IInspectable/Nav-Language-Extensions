#region Using Directives

using Microsoft.VisualStudio.Core.Imaging;
using Microsoft.VisualStudio.Text.Adornments;

using Pharmatechnik.Nav.Language.Extension.Images;

#endregion

namespace Pharmatechnik.Nav.Language.Extension.Completion; 

static class CompletionImages {

    public static ImageElement Keyword      = new(ImageMonikers.Keyword.ToImageId());
    public static ImageElement Folder       = new(ImageMonikers.FolderClosed.ToImageId());
    public static ImageElement NavFile      = new(ImageMonikers.Include.ToImageId());
    public static ImageElement File         = new(ImageMonikers.File.ToImageId());
    public static ImageElement ParentFolder = new(ImageMonikers.ParentFolder.ToImageId());

    public static ImageElement Choice          = new(ImageMonikers.ChoiceNode.ToImageId());
    public static ImageElement Task            = new(ImageMonikers.TaskNode.ToImageId());
    public static ImageElement GuiNode         = new(ImageMonikers.ViewNode.ToImageId());
    public static ImageElement ConnectionPoint = new(ImageMonikers.ExitConnectionPoint.ToImageId());

    public static ImageElement FromSymbol(ISymbol symbol) => new(ImageMonikers.FromSymbol(symbol).ToImageId());
}